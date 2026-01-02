using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetDeletedBudgetById;

public class GetDeletedBudgetByIdQueryHandler : IRequestHandler<GetDeletedBudgetByIdQuery, BudgetDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetDeletedBudgetByIdQueryHandler(
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<BudgetDto> Handle(GetDeletedBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var budget = await _budgetRepository.GetDeletedBudgetAsync(request.Id, userId, cancellationToken);
        if (budget == null)
            throw new NotFoundException(nameof(BudgetDto), request.Id);

        return _mapper.Map<BudgetDto>(budget);
    }
}