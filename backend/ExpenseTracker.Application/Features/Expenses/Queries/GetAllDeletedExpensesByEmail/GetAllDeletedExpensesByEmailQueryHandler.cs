using AutoMapper;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllDeletedExpensesByEmail;

public class GetAllDeletedExpensesByEmailQueryHandler 
    : IRequestHandler<GetAllDeletedExpensesByEmailQuery, PagedResult<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetAllDeletedExpensesByEmailQueryHandler(
        IExpenseRepository expenseRepository, 
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

     public async Task<PagedResult<ExpenseDto>> Handle(
        GetAllDeletedExpensesByEmailQuery request,
        CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can view their own deleted expenses

        var userId = _userAccessor.UserId;
        
        var query = request.Paging;

        var(softDeletedExpenses, totalCount) = await _expenseRepository.GetAllDeletedExpensesByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(softDeletedExpenses);
        return new PagedResult<ExpenseDto>(mappedExpenses, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}
