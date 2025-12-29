using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.DeleteExpense;

public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Unit>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserAccessor _userAccessor;

    public DeleteExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        IUserAccessor userAccessor)
    {
        _expenseRepository = expenseRepository;
        _userAccessor = userAccessor;
    }       

    public async Task<Unit> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can delete their expenses
        
        var userId = _userAccessor.UserId;

        var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (expense == null)
            throw new NotFoundException(nameof(Expense), request.Id);

        // var ownsExpense = await _expenseRepository.UserOwnsExpenseAsync(request.Id, userId, cancellationToken);
        // if (!ownsExpense)
        // {
        //     throw new BadRequestException($"You don't have Expense with id '{request.Id}'");
        // }

        if(expense.UserId != userId)
            throw new ForbiddenException($"You don't have access to delete expense with id '{request.Id}'.");

        await _expenseRepository.DeleteAsync(expense, cancellationToken);
        return Unit.Value;
    }
}
