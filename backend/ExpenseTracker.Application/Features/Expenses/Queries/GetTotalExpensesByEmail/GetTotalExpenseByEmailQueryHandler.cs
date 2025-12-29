using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetTotalExpensesByEmail;

public class GetTotalExpenseByEmailQueryHandler : IRequestHandler<GetTotalExpenseByEmailQuery, TotalExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;

    public GetTotalExpenseByEmailQueryHandler(
        IExpenseRepository expenseRepository,
        IUserAccessor userAccessor,
        IUserRepository userRepository)
    {
        _expenseRepository = expenseRepository;
        _userAccessor = userAccessor;
        _userRepository = userRepository;   
    }

    public async Task<TotalExpenseDto> Handle(GetTotalExpenseByEmailQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var totalAmount = await _expenseRepository.GetTotalExpenseByEmailAsync(userId, cancellationToken);
        return new TotalExpenseDto { TotalAmount = totalAmount };
    }
}