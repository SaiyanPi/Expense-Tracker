using AutoMapper;
using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Cache;
using ExpenseTracker.Application.DTOs.Dashboard;
using ExpenseTracker.Application.DTOS.Dashboard;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Dashboard.Query;

public class GetMonthlyDashboardQueryHandler : IRequestHandler<GetMonthlyDashboardQuery, DashboardSummaryDto>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetMonthlyDashboardQueryHandler> _logger;

    public GetMonthlyDashboardQueryHandler(
        IDashboardRepository dashboardRepository,
        IUserAccessor userAccessor,
        IMapper mapper,
        IMemoryCache cache,
        ILogger<GetMonthlyDashboardQueryHandler> logger)
    {
        _dashboardRepository = dashboardRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetMonthlyDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        var now = DateTime.UtcNow;
        var startDate = new DateTime(now.Year, now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1); // get the last date of the month 2025-12-31
        // var endDate = startDate.AddMonths(1);    // get the start pf the next month 2026-01-01

        // Check cache first
        var cacheKey = CacheKeys.Dashboard(userId, now.Year, now.Month);
        if (_cache.TryGetValue(cacheKey, out DashboardSummaryDto? cachedDashboard) && cachedDashboard != null)
        {
            _logger.LogInformation("Dashboard from In-memory cache");
            CacheMetrics.RecordHit();   // record cache hit metric
            return cachedDashboard;
        }

        CacheMetrics.RecordMiss();  // record cache miss metric

        var totalExpenses = await _dashboardRepository.GetTotalExpensesForMonthAsync(userId, startDate, endDate, cancellationToken);
        var totalBudgets = await _dashboardRepository.GetTotalBudgetForMonthAsync(userId, startDate, endDate, cancellationToken);
        var expensesByCategory = await _dashboardRepository.GetExpensesByCategoryForMonthAsync(userId, startDate, endDate, cancellationToken);
        var dailyExpenses = await _dashboardRepository.GetDailyExpensesForMonthAsync(userId, startDate, endDate, cancellationToken);
        var recentExpenses = await _dashboardRepository.GetRecentExpensesForMonthAsync(userId, startDate, endDate, 5, cancellationToken);

        var mappedDashboardCategoryExpenseSummary = _mapper.Map<List<CategoryExpenseDto>>(expensesByCategory);
        var mappedDashboardDailyExpenseSummary = _mapper.Map<List<DailyExpenseDto>>(dailyExpenses);
        var mappedDashboardRecentExpense = _mapper.Map<List<RecentExpenseDto>>(recentExpenses);

        var topCategory = expensesByCategory
            .OrderByDescending(c => c.TotalAmount)
            .Select(c => new CategoryExpenseDto { Category = c.Category, TotalAmount = c.TotalAmount })
            .FirstOrDefault();

        var dashboard = new DashboardSummaryDto
        {
            TotalExpenses = totalExpenses,
            TotalBudgets = totalBudgets,
            RemainingBudget = totalBudgets > 0 ? totalBudgets - totalExpenses : null,
            TopCategory = topCategory,
            ExpenseByCategory = mappedDashboardCategoryExpenseSummary,
            DailyExpenses = mappedDashboardDailyExpenseSummary,
            RecentExpenses = mappedDashboardRecentExpense
        };

        // cache the result
        var cacheEntryOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, dashboard, cacheEntryOption);

        _logger.LogInformation("Dashboard from database");
        return dashboard;
    }
}