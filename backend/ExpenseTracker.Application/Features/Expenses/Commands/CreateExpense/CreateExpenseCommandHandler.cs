using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Commands.CreateExpense;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        // check if the category exist
        var category = await _categoryRepository.GetByIdAsync(request.CreateExpenseDto.CategoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), request.CreateExpenseDto.CategoryId);

        // check if the user exist
        if (request.CreateExpenseDto.UserId is not null)
        {
            var user = await _userRepository.GetByIdAsync(request.CreateExpenseDto.UserId, cancellationToken);
            if (user is null)
                throw new NotFoundException(nameof(User), request.CreateExpenseDto.UserId);
        }
        var expense = _mapper.Map<Expense>(request.CreateExpenseDto);
        await _expenseRepository.AddAsync(expense, cancellationToken);
        return _mapper.Map<ExpenseDto>(expense);
    }
}