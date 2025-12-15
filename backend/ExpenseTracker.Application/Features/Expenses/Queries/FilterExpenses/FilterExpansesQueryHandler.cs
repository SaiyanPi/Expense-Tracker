using AutoMapper;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.FilterExpenses;

public class FilterExpenseQueryHandler : IRequestHandler<FilterExpensesQuery, FilteredExpensesResultDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;

    public FilterExpenseQueryHandler(
        IExpenseRepository expenseRepository,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<FilteredExpensesResultDto> Handle(FilterExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = request.Paging;

        var filteredExpenses = await _expenseRepository.GetFilterExpensesAsync(
            request.StartDate,
            request.EndDate,
            request.MinAmount,
            request.MaxAmount,
            request.CategoryId,
            request.UserId,

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