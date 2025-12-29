using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;

public class GetAllExpensesByEmailQueryHandler : IRequestHandler<GetAllExpensesByEmailQuery, PagedResult<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetAllExpensesByEmailQueryHandler(
        IExpenseRepository expenseRepository,
        IUserRepository userRepository, 
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<PagedResult<ExpenseDto>> Handle(
        GetAllExpensesByEmailQuery request,
        CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can view their own expenses

        var userId = _userAccessor.UserId;
        
        var query = request.Paging;

        var(expenses, totalCount) = await _expenseRepository.GetExpensesByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(expenses);
        return new PagedResult<ExpenseDto>(mappedExpenses, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}