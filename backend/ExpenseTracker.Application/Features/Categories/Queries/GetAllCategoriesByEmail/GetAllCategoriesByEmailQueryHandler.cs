using AutoMapper;
using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Cache;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetAllCategoriesByEmail;

public class GetAllCategoriesByEmailQueryHandler : IRequestHandler<GetAllCategoriesByEmailQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetAllCategoriesByEmailQueryHandler> _logger;

    public GetAllCategoriesByEmailQueryHandler(
        ICategoryRepository categoryRepository, 
        IUserRepository userRepository,
        IUserAccessor userAccessor, 
        IMapper mapper,
        IMemoryCache cache,
        ILogger<GetAllCategoriesByEmailQueryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<CategoryDto>> Handle(
        GetAllCategoriesByEmailQuery request, 
        CancellationToken cancellationToken)
    {        
        // BUISNESS RULE:
        // Only users can view their own categories
        
        var userId = _userAccessor.UserId;

        var query = request.Paging;

        // Check cache first
        var now = DateTime.UtcNow;
        var cacheKey = CacheKeys.Expense(userId, now.Year, now.Month);
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<CategoryDto>? cachedMappedCategories)
            && cachedMappedCategories != null)
        {
            _logger.LogInformation("User Categories from In-memory cache");

            CacheMetrics.RecordHit();   // record cache hit metric

            var totalCategories = cachedMappedCategories.Count;
            return new PagedResult<CategoryDto>(
                cachedMappedCategories,
                totalCategories,
                query.EffectivePage,
                query.EffectivePageSize);
        }

        CacheMetrics.RecordMiss();  // record cache miss metric

        var (categories, totalCount) = await _categoryRepository.GetAllCategoriesByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedCategories = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);

        // cache the result
        var cacheEntryOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, mappedCategories, cacheEntryOption);

        _logger.LogInformation("User Categories from database");

        return new PagedResult<CategoryDto>(
            mappedCategories,
            totalCount,
            query.EffectivePage,
            query.EffectivePageSize);
    }
}