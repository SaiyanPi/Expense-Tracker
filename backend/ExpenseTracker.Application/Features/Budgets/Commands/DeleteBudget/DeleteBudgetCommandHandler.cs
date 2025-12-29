using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Commands.DeleteBudget;

public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, Unit>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;


    public DeleteBudgetCommandHandler(
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor)
    {
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
    }

    public async Task<Unit> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        // BUSINESS RULE:
        // Delete when: budget with or without expense(s) is inactive and active budget without expense(s)
        // NO DELETE when: budget is active and has expense(s)

        var userId = _userAccessor.UserId;

        var budget = await _budgetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (budget == null)
            throw new NotFoundException(nameof(Budget), request.Id);

        if(budget.UserId != userId)
            throw new ForbiddenException($"You don't have access to delete budget with id '{request.Id}'.");

        // check if the budget is active and has expense(s)
        if(budget.IsActive == true && budget.Expenses.Any())
            throw new BadRequestException("Active budgets with existing expenses cannot be deleted.");

        await _budgetRepository.DeleteAsync(budget, cancellationToken);
        return Unit.Value;
    }
}