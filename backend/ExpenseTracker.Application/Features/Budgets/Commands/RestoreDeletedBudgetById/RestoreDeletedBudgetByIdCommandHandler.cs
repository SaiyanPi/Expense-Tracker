using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Commands.RestoreDeletedBudgetById;

public class RestoreDeletedBudgetByIdCommandHandler 
    : IRequestHandler<RestoreDeletedBudgetByIdCommand, Unit>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUserAccessor _userAccessor;

    public RestoreDeletedBudgetByIdCommandHandler(
        IBudgetRepository budgetRepository,
        IUserAccessor userAccessor)
    {
        _budgetRepository = budgetRepository;
        _userAccessor = userAccessor;
    }       

    public async Task<Unit> Handle(RestoreDeletedBudgetByIdCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can restore their deleted budgets
        
        var userId = _userAccessor.UserId;

        // fetch deleted Budget by id
        var deletedBudget = await _budgetRepository.GetDeletedBudgetAsync(request.Id, userId, cancellationToken);
        if (deletedBudget == null)
            throw new NotFoundException(nameof(Budget), request.Id);

        // restore and save
        deletedBudget.IsDeleted = false;
        deletedBudget.DeletedAt = null;
        deletedBudget.DeletedBy = null;
        var restored = await _budgetRepository.RestoreDeletedBudgetAsync();
        if (!restored)
            throw new BadRequestException("restore failed!");
        return Unit.Value;

    }
}
