using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.RestoreDeletedExpenseById;

public class RestoreDeletedExpenseByIdCommandHandler 
    : IRequestHandler<RestoreDeletedExpenseByIdCommand, Unit>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserAccessor _userAccessor;

    public RestoreDeletedExpenseByIdCommandHandler(
        IExpenseRepository expenseRepository,
        IUserAccessor userAccessor)
    {
        _expenseRepository = expenseRepository;
        _userAccessor = userAccessor;
    }       

    public async Task<Unit> Handle(RestoreDeletedExpenseByIdCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can restore their deleted expenses
        
        var userId = _userAccessor.UserId;

        // fetch deleted Expense by id
        var deletedExpense = await _expenseRepository.GetDeletedExpenseAsync(request.Id, userId, cancellationToken);
        if (deletedExpense == null)
            throw new NotFoundException(nameof(Expense), request.Id);

        // restore and save
        deletedExpense.IsDeleted = false;
        deletedExpense.DeletedAt = null;
        deletedExpense.DeletedBy = null;
        var restored = await _expenseRepository.RestoreDeletedExpenseAsync();
        if (!restored)
            throw new BadRequestException("restore failed!");
        return Unit.Value;

    }
}
