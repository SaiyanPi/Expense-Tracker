using AutoMapper;
using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Cache;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllActiveBudgetsByEmail;

public class GetAllActiveBudgetsByEmailQueryHandler : IRequestHandler<GetAllActiveBudgetsByEmailQuery, PagedResult<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetAllActiveBudgetsByEmailQueryHandler> _logger;
    private readonly ICacheVersionService _cacheVersionService;


    public GetAllActiveBudgetsByEmailQueryHandler
    (
        IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IMapper mapper,
        IMemoryCache cache,
        ICacheVersionService cacheVersionService,
        ILogger<GetAllActiveBudgetsByEmailQueryHandler> logger)
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
        _cacheVersionService = cacheVersionService;
    }

    public async Task<PagedResult<BudgetDto>> Handle(
        GetAllActiveBudgetsByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var query = request.Paging;

        // determining the cache version for the user, if the version is not found in the cache,
        // it will be initialized to 1
        var version = _cacheVersionService.GetVersion(CacheGroups.Budgets, userId);

        // Check cache first
        var now = DateTime.UtcNow;
        var cacheKey = CacheKeys.Budget(userId, version, now.Year, now.Month, query.EffectivePage,
            query.EffectivePageSize, query.SortBy, query.SortDesc);

        if (_cache.TryGetValue(cacheKey, out PagedResult<BudgetDto>? cachedResult)
            && cachedResult != null)
        {
            _logger.LogInformation("User Active Budgets from In-memory cache");

            CacheMetrics.RecordHit();   // record cache hit metric

            return cachedResult;
        }

        CacheMetrics.RecordMiss();  // record cache miss metric

        var (budgets, totalCount) = await _budgetRepository.GetAllActiveBudgetsByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedBudgets = _mapper.Map<IReadOnlyList<BudgetDto>>(budgets);

        var result = new PagedResult<BudgetDto>(
            mappedBudgets,
            totalCount,
            query.EffectivePage,
            query.EffectivePageSize);

        // cache the result
        var cacheEntryOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, result, cacheEntryOption);

        _logger.LogInformation("User Budgets from database");

        return result;
    }
}