using ExpenseTracker.Application.Common.Caching;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Observability.Metrics.Business.DomainSpecific;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Expenses.Commands.DeleteExpense;

public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Unit>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly ILogger<DeleteExpenseCommandHandler> _logger;
    private readonly ICacheVersionService _cacheVersionService;

    public DeleteExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        IUserAccessor userAccessor,
        ICacheVersionService cacheVersionService,
        ILogger<DeleteExpenseCommandHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _userAccessor = userAccessor;
        _logger = logger;
        _cacheVersionService = cacheVersionService;
    }       

    public async Task<Unit> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        _logger.LogInformation(
            "Deleting expense with id {ExpenseId} for UserId {UserId}",
            request.Id,
            userId
        );

         // BUISNESS RULE:
        // Only user can delete their expenses

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

        // Invalidate the cache once a new expense is deleted for the user, so that the deleted
        // query will fetch fresh data
        _cacheVersionService.IncrementVersion(CacheGroups.Expenses, userId);

        // hook the business metric
        ExpenseMetrics.ExpenseDeleted();

        _logger.LogInformation(
            "Expense deleted successfully with id {ExpenseId} for UserId {UserId}",
            request.Id,
            userId
        );

        return Unit.Value;
    }
}
