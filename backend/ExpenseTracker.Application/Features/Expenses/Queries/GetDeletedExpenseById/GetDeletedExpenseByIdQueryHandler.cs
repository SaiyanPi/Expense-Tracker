using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetDeletedExpenseById;

public class GetDeletedExpenseByIdQueryHandler : IRequestHandler<GetDeletedExpenseByIdQuery, ExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetDeletedExpenseByIdQueryHandler(
        IExpenseRepository expenseRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<ExpenseDto> Handle(GetDeletedExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var expense = await _expenseRepository.GetDeletedExpenseAsync(request.Id, userId, cancellationToken);
        if (expense == null)
            throw new NotFoundException(nameof(ExpenseDto), request.Id);

        return _mapper.Map<ExpenseDto>(expense);
    }
}