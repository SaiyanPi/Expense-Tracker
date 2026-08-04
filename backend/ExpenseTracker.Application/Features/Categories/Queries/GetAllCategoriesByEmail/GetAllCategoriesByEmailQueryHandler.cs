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
    private readonly ICacheVersionService _cacheVersionService;

    public GetAllCategoriesByEmailQueryHandler(
        ICategoryRepository categoryRepository, 
        IUserRepository userRepository,
        IUserAccessor userAccessor, 
        IMapper mapper,
        IMemoryCache cache,
        ICacheVersionService cacheVersionService,
        ILogger<GetAllCategoriesByEmailQueryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cacheVersionService = cacheVersionService;
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

        // determining the cache version for the user, if the version is not found in the cache,
        // it will be initialized to 1
            // var versionKey = CacheKeys.CategoryVersion(userId);
            // if (!_cache.TryGetValue(versionKey, out int version))
            // {
            //     version = 1;
            //     _cache.Set(versionKey, version);
            // }
        
        //var version = _categoryCacheVersionService.GetVersion(userId);
        var version = _cacheVersionService.GetVersion(CacheGroups.Categories, userId);

        // Check cache first
        var now = DateTime.UtcNow;
        var cacheKey = CacheKeys.Category(userId, version, now.Year, now.Month, query.EffectivePage,
            query.EffectivePageSize, query.SortBy, query.SortDesc);

        if (_cache.TryGetValue(cacheKey, out PagedResult<CategoryDto>? cachedResult)
            && cachedResult != null)
        {
            _logger.LogInformation("User Categories from In-memory cache");

            CacheMetrics.RecordHit();   // record cache hit metric

            return cachedResult;
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

        var result = new PagedResult<CategoryDto>(
            mappedCategories,
            totalCount,
            query.EffectivePage,
            query.EffectivePageSize);
        
         // cache the result
        var cacheEntryOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, result, cacheEntryOption);

        _logger.LogInformation("User Categories from database");

        return result;
    }
}