using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Business.DomainSpecific;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Budgets.Commands.DeleteBudget;

public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, Unit>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly ILogger<DeleteBudgetCommandHandler> _logger;


    public DeleteBudgetCommandHandler(
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor,
        ILogger<DeleteBudgetCommandHandler> logger)
    {
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        _logger.LogInformation(
            "Deleting budget with id {BudgetId} and UserId {UserId} ",
            request.Id,
            userId
        );
    
        // BUSINESS RULE:
        // Delete when: budget with or without expense(s) is inactive and active budget without expense(s)
        // NO DELETE when: budget is active and has expense(s)

        var budget = await _budgetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (budget == null)
            throw new NotFoundException(nameof(Budget), request.Id);

        if(budget.UserId != userId)
            throw new ForbiddenException($"You don't have access to delete budget with id '{request.Id}'.");

        // check if the budget is active and has expense(s)
        if(budget.IsActive == true && budget.Expenses.Any())
            throw new BadRequestException("Active budgets with existing expenses cannot be deleted.");

        await _budgetRepository.DeleteAsync(budget, cancellationToken);

        // hook the business metric
        BudgetMetrics.BudgetDeleted();
        
        _logger.LogInformation(
            "Budget deleted successfully with id {BudgetId} and UserId {UserId}.",
            request.Id,
            userId
        );

        return Unit.Value;
    }
}