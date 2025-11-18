using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesByEmail;

public class GetAllExpensesByEmailQueryHandler : IRequestHandler<GetAllExpensesByEmailQuery, IReadOnlyList<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllExpensesByEmailQueryHandler(IExpenseRepository expenseRepository,
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ExpenseDto>> Handle(GetAllExpensesByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.Email);
            
        var expenses = await _expenseRepository.GetAllExpensesByEmailAsync(user.Id, cancellationToken);
        return _mapper.Map<IReadOnlyList<ExpenseDto>>(expenses);
    }
}