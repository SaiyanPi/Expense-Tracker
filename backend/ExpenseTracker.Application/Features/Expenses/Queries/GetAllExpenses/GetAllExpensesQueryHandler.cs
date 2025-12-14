using AutoMapper;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpenses;

public class GetAllExpensesQueryHandler : IRequestHandler<GetAllExpensesQuery, PagedResult<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;

    public GetAllExpensesQueryHandler(
        IExpenseRepository expenseRepository,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ExpenseDto>> Handle(
        GetAllExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var query = request.Paging;

        var(expenses, totalCount) = await _expenseRepository.GetAllAsync(
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);

        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(expenses);
        return new PagedResult<ExpenseDto>(mappedExpenses, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}