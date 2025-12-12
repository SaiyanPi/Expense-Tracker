using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForCategoryByEmail;

public class GetAllExpensesForCategoryByEmailQueryHandler : IRequestHandler<GetAllExpensesForCategoryByEmailQuery, IReadOnlyList<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetAllExpensesForCategoryByEmailQueryHandler
        (IExpenseRepository expenseRepository, 
        IUserRepository userRepository, 
        ICategoryRepository categoryRepository, 
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ExpenseDto>> Handle(GetAllExpensesForCategoryByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.Email);
   
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var expensesForCategory = await _expenseRepository.GetExpensesForACategoryByEmailAsync(category.Id, user.Id, cancellationToken);

        return _mapper.Map<IReadOnlyList<ExpenseDto>>(expensesForCategory);
    }
}