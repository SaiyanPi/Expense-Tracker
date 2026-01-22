using AutoMapper;
using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Cache;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;

public class GetAllExpensesByEmailQueryHandler : IRequestHandler<GetAllExpensesByEmailQuery, PagedResult<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetAllExpensesByEmailQueryHandler> _logger;

    public GetAllExpensesByEmailQueryHandler(
        IExpenseRepository expenseRepository,
        IUserRepository userRepository, 
        IUserAccessor userAccessor,
        IMapper mapper,
        IMemoryCache cache,
        ILogger<GetAllExpensesByEmailQueryHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<ExpenseDto>> Handle(
        GetAllExpensesByEmailQuery request,
        CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can view their own expenses

        var userId = _userAccessor.UserId;
        
        var query = request.Paging;

        // Check cache first
        var now = DateTime.UtcNow;
        var cacheKey = CacheKeys.Expense(userId, now.Year, now.Month);
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<ExpenseDto>? cachedMappedExpenses)
            && cachedMappedExpenses != null)
        {
            _logger.LogInformation("User Expenses from In-memory cache");

            CacheMetrics.RecordHit();   // record cache hit metric

            var totalExpenses = cachedMappedExpenses.Count;
            return new PagedResult<ExpenseDto>(
                cachedMappedExpenses,
                totalExpenses,
                query.EffectivePage,
                query.EffectivePageSize);
        }

        CacheMetrics.RecordMiss();  // record cache miss metric

        var(expenses, totalCount) = await _expenseRepository.GetExpensesByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(expenses);

        // cache the result
        var cacheEntryOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, mappedExpenses, cacheEntryOption);

        _logger.LogInformation("User Expenses from database");

        return new PagedResult<ExpenseDto>(
            mappedExpenses,
            totalCount,
            query.EffectivePage,
            query.EffectivePageSize);
    }
}