using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;

public class FilterExpenseQueryHandler : IRequestHandler<FilterExpensesQuery, FilteredExpensesResultDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepopsitory;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserRoleService _userRoleService;
    private readonly IMapper _mapper;

    public FilterExpenseQueryHandler(
        IExpenseRepository expenseRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userRepopsitory = userRepository;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
        _mapper = mapper;
    }

    public async Task<FilteredExpensesResultDto> Handle(FilterExpensesQuery request, CancellationToken cancellationToken)
    {
        // If the user is admin, admin can filter with different userId
        // if the user is regular, regular user cannot filter with userId. 
        // Default userId will the their own id for regular id.
        if (request.UserId != null)
        {
            var userExist = await _userRepopsitory.GetByIdAsync(request.UserId, cancellationToken);
            if(userExist is null)
                throw new NotFoundException(nameof(User), request.UserId);
        }

        var userId = _userAccessor.UserId;

        var isAdmin = await _userRoleService.IsAdminAsync(userId);
        
        var effectiveUserId = isAdmin ? request.UserId : userId;

        var query = request.Paging;

        var filteredExpenses = await _expenseRepository.GetFilterExpensesAsync(
            request.StartDate,
            request.EndDate,
            request.MinAmount,
            request.MaxAmount,
            request.CategoryId,
            effectiveUserId,

            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);

        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(filteredExpenses.Expenses);
        var pagedExpenses = new PagedResult<ExpenseDto>(
            mappedExpenses,
            filteredExpenses.TotalCount,
            query.EffectivePage,
            query.EffectivePageSize);

        return new FilteredExpensesResultDto
        {
            TotalAmount = filteredExpenses.TotalAmount,
            Expenses = pagedExpenses
        };
    }
}