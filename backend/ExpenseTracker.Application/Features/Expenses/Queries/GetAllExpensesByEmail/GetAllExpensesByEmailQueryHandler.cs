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
    private readonly ICacheVersionService _cacheVersionService;

    public GetAllExpensesByEmailQueryHandler(
        IExpenseRepository expenseRepository,
        IUserRepository userRepository, 
        IUserAccessor userAccessor,
        IMapper mapper,
        IMemoryCache cache,
        ICacheVersionService cacheVersionService,
        ILogger<GetAllExpensesByEmailQueryHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
        _cacheVersionService = cacheVersionService;
    }

    public async Task<PagedResult<ExpenseDto>> Handle(
        GetAllExpensesByEmailQuery request,
        CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can view their own expenses

        var userId = _userAccessor.UserId;
        
        var query = request.Paging;

        // determining the cache version for the user, if the version is not found in the cache,
        // it will be initialized to 1
        var version = _cacheVersionService.GetVersion(CacheGroups.Expenses, userId);

        // Check cache first
        var now = DateTime.UtcNow;
        var cacheKey = CacheKeys.Expense(userId, version, now.Year, now.Month, query.EffectivePage,
            query.EffectivePageSize, query.SortBy, query.SortDesc);

        if (_cache.TryGetValue(cacheKey, out PagedResult<ExpenseDto>? cachedResult)
            && cachedResult != null)
        {
            _logger.LogInformation("User Expenses from In-memory cache");

            CacheMetrics.RecordHit();   // record cache hit metric

            return cachedResult;
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

        var result = new PagedResult<ExpenseDto>(
            mappedExpenses,
            totalCount,
            query.EffectivePage,
            query.EffectivePageSize);

        // cache the result
        var cacheEntryOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, result, cacheEntryOption);

        _logger.LogInformation("User Expenses from database");

        return result;
    }
}