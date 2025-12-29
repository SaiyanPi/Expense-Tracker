using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetById;

public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, BudgetDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetBudgetByIdQueryHandler(
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<BudgetDto> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var budget =  await _budgetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (budget is null)
        {
            throw new NotFoundException(nameof(Budget), request.Id);
        }

        if(budget.UserId != userId)
            throw new ForbiddenException($"You don't have access to budget '{request.Id}'.");

        return _mapper.Map<BudgetDto>(budget);
    }
}