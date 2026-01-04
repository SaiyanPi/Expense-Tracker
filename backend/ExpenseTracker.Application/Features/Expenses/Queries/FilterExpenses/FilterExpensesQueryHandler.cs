using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;

public class FilterExpenseQueryHandler : IRequestHandler<FilterExpensesQuery, FilteredExpensesResultDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;
    private readonly IMapper _mapper;

    public FilterExpenseQueryHandler(
        IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
        _mapper = mapper;
    }

    public async Task<FilteredExpensesResultDto> Handle(FilterExpensesQuery request, CancellationToken cancellationToken)
    {
        // If the user is admin, admin can filter with different userId
        // if the user is regular, regular user cannot filter with userId. 
        // Default userId will the their own id for regular id.
        // if (request.UserId != null)
        // {
        //     var userExist = await _userRepopsitory.GetByIdAsync(request.UserId, cancellationToken);
        //     if(userExist is null)
        //         throw new NotFoundException(nameof(User), request.UserId);
        // }

        // var userId = _userAccessor.UserId;

        // var isAdmin = await _userRoleService.IsAdminAsync(userId);
        
        // var effectiveUserId = isAdmin ? request.UserId : userId;

        // var page = request.Paging;

        // var filteredExpenses = await _expenseRepository.GetFilteredExpensesAsync(
        //     request.StartDate,
        //     request.EndDate,
        //     request.MinAmount,
        //     request.MaxAmount,
        //     request.CategoryId,
        //     effectiveUserId,

        //     skip: page.Skip,
        //     take: page.EffectivePageSize,
        //     sortBy: page.SortBy,
        //     sortDesc: page.SortDesc,
        //     cancellationToken: cancellationToken);

        // var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(filteredExpenses.Expenses);
        // var pagedExpenses = new PagedResult<ExpenseDto>(
        //     mappedExpenses,
        //     filteredExpenses.TotalCount,
        //     page.EffectivePage,
        //     page.EffectivePageSize);

        // return new FilteredExpensesResultDto
        // {
        //     TotalAmount = filteredExpenses.TotalAmount,
        //     Expenses = pagedExpenses
        // };



        var userId = _userAccessor.UserId;
        var isAdmin = await _userRoleService.IsAdminAsync(userId);
        
        var effectiveUserId = isAdmin ? request.Filter.UserId : userId;

        // Validate only if admin provided a userId
        if (isAdmin && !string.IsNullOrWhiteSpace(effectiveUserId))
        {
            var user = await _userRepository.GetByIdAsync(effectiveUserId);
            if (user is null)
                throw new NotFoundException(nameof(User), effectiveUserId);
        }

        if (request.Filter.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Filter.CategoryId.Value);
            if(category is null)
                throw new NotFoundException(nameof(Category), request.Filter.CategoryId.Value);
        }
        

        // build query
        var filter = request.Filter;
        var page = request.Paging;

        IQueryable<Expense> query = _expenseRepository.GetExpensesQueryable();

        if (filter.StartDate.HasValue)
            query = query.Where(e => e.Date >= filter.StartDate.Value);
    
        if (filter.EndDate.HasValue)
            query = query.Where(e => e.Date <= filter.EndDate.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(e => e.Amount >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(e => e.Amount <= filter.MaxAmount.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(e => e.CategoryId == filter.CategoryId.Value);

        if (!string.IsNullOrEmpty(effectiveUserId))
            query = query.Where(e => e.UserId == effectiveUserId);

        // Sorting
        query = query.ApplySorting(page.SortBy, page.SortDesc);

        // Pagination
        var totalCount = await query.CountAsync(cancellationToken);
        
        var filteredExpenses = await query
            .Skip(page.Skip)
            .Take(page.EffectivePageSize)
            .ToListAsync(cancellationToken);

        var mappedFilteredExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(filteredExpenses);

        var pagedFilteredExpenses = new PagedResult<ExpenseDto>(
            mappedFilteredExpenses,
            totalCount,
            page.EffectivePage,
            page.EffectivePageSize);

        return new FilteredExpensesResultDto
        {
            TotalAmount = filteredExpenses.Sum(e => e.Amount),
            Expenses = pagedFilteredExpenses
        };

    }
}