using AutoMapper;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetAllBudgets;

public class GetAllBudgetQueryHandler : IRequestHandler<GetAllBudgetQuery, IReadOnlyList<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IMapper _mapper;

    public GetAllBudgetQueryHandler(IBudgetRepository budgetRepository, IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BudgetDto>> Handle(GetAllBudgetQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _budgetRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<BudgetDto>>(budgets);
    }
}