using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForCategoryByEmail;

public class GetAllExpensesForCategoryByEmailQueryHandler : IRequestHandler<GetAllExpensesForCategoryByEmailQuery, PagedResult<ExpenseDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetAllExpensesForCategoryByEmailQueryHandler(
        IExpenseRepository expenseRepository, 
        IUserRepository userRepository, 
        ICategoryRepository categoryRepository, 
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ExpenseDto>> Handle(
        GetAllExpensesForCategoryByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.Email);
   
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var query = request.Paging;
        
        var (expenses, totalCount) = await _expenseRepository.GetExpensesForACategoryByEmailAsync(
            category.Id,
            user.Id,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseDto>>(expenses);
        return new PagedResult<ExpenseDto>(mappedExpenses, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}