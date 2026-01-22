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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgetsByEmail;

public class GetAllBudgetsByEmailQueryHandler : IRequestHandler<GetAllBudgetsByEmailQuery, PagedResult<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetAllBudgetsByEmailQueryHandler> _logger;


    public GetAllBudgetsByEmailQueryHandler
    (
        IBudgetRepository budgetRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IMapper mapper,
        IMemoryCache cache,
        ILogger<GetAllBudgetsByEmailQueryHandler> logger)
    {
        _budgetRepository = budgetRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<BudgetDto>> Handle(
        GetAllBudgetsByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var query = request.Paging;

        // Check cache first
        var now = DateTime.UtcNow;
        var cacheKey = CacheKeys.Expense(userId, now.Year, now.Month);
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<BudgetDto>? cachedMappedBudgets)
            && cachedMappedBudgets != null)
        {
            _logger.LogInformation("User Budgets from In-memory cache");

            CacheMetrics.RecordHit();   // record cache hit metric

            var totalCategories = cachedMappedBudgets.Count;
            return new PagedResult<BudgetDto>(
                cachedMappedBudgets,
                totalCategories,
                query.EffectivePage,
                query.EffectivePageSize);
        }

        CacheMetrics.RecordMiss();  // record cache miss metric

        var (budgets, totalCount) = await _budgetRepository.GetAllBudgetsByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedBudgets = _mapper.Map<IReadOnlyList<BudgetDto>>(budgets);

        // cache the result
        var cacheEntryOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, mappedBudgets, cacheEntryOption);

        _logger.LogInformation("User Budgets from database");

        return new PagedResult<BudgetDto>(
            mappedBudgets,
            totalCount,
            query.EffectivePage,
            query.EffectivePageSize);
    }
}