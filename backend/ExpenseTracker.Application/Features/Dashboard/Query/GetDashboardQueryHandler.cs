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

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardSummaryDto>
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ICacheVersionService _cacheVersionService;
    private readonly ILogger<GetDashboardQueryHandler> _logger;

    public GetDashboardQueryHandler(
        IDashboardRepository dashboardRepository,
        IUserAccessor userAccessor,
        IMapper mapper,
        IMemoryCache cache,
        ICacheVersionService cacheVersionService,
        ILogger<GetDashboardQueryHandler> logger)
    {
        _dashboardRepository = dashboardRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cache = cache;
        _cacheVersionService = cacheVersionService;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        var now = DateTime.UtcNow;
        var startDate = request.StartDate.Date;
        var endDate = request.EndDate.Date.AddDays(1); 

        var version = _cacheVersionService.GetVersion(CacheGroups.Dashboard, userId);

        // Check cache first
        var cacheKey = CacheKeys.Dashboard(userId,version, request.StartDate, request.EndDate);
        if (_cache.TryGetValue(cacheKey, out DashboardSummaryDto? cachedResult) && cachedResult != null)
        {
            _logger.LogInformation("Dashboard from In-memory cache");
            CacheMetrics.RecordHit();   // record cache hit metric
            return cachedResult;
        }

        CacheMetrics.RecordMiss();  // record cache miss metric

        var totalExpenses = await _dashboardRepository.GetTotalExpensesAsync(userId, startDate, endDate, cancellationToken);
        var totalBudgets = await _dashboardRepository.GetTotalBudgetAsync(userId, startDate, endDate, cancellationToken);
        var expensesByCategory = await _dashboardRepository.GetExpensesByCategoryAsync(userId, startDate, endDate, cancellationToken);
        var dailyExpenses = await _dashboardRepository.GetDailyExpensesAsync(userId, startDate, endDate, cancellationToken);
        var recentExpenses = await _dashboardRepository.GetRecentExpensesAsync(userId, startDate, endDate, 5, cancellationToken);

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