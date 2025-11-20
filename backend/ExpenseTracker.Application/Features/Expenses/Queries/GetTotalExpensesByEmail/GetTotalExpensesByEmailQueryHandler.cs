using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetTotalExpensesByEmail;

public class GetTotalExpensesByEmailQueryHandler : IRequestHandler<GetTotalExpensesByEmailQuery, TotalExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;

    public GetTotalExpensesByEmailQueryHandler(IExpenseRepository expenseRepository,
        IUserRepository userRepository)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;   
    }

    public async Task<TotalExpenseDto> Handle(GetTotalExpensesByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(TotalExpenseDto), request.Email);
   
        var totalAmount = await _expenseRepository.GetTotalExpensesByEmailAsync(user.Id, cancellationToken);
        return new TotalExpenseDto { TotalAmount = totalAmount };
    }
}