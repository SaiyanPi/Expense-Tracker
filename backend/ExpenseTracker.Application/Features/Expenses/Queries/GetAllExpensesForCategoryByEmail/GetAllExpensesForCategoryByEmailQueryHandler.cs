using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.DTOS.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetAllExpensesForCategoryByEmail;

public class GetAllExpensesForCategoryByEmailQueryHandler : IRequestHandler<GetAllExpensesForCategoryByEmailQuery, PagedResult<ExpenseSummaryForCategoryDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetAllExpensesForCategoryByEmailQueryHandler(
        IExpenseRepository expenseRepository, 
        IUserRepository userRepository, 
        ICategoryRepository categoryRepository, 
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<PagedResult<ExpenseSummaryForCategoryDto>> Handle(
        GetAllExpensesForCategoryByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var query = request.Paging;
        
        var (expenses, totalCount) = await _expenseRepository.GetExpensesForACategoryByEmailAsync(
            category.Id,
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseSummaryForCategoryDto>>(expenses);
        return new PagedResult<ExpenseSummaryForCategoryDto>(mappedExpenses, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}