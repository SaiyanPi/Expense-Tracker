using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, BudgetDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateBudgetCommandHandler(
        IBudgetRepository budgetRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper
    )
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<BudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        if(request.CreateBudgetDto.CategoryId is not null)
        {
            var categoryExists = await _categoryRepository.GetByIdAsync(request.CreateBudgetDto.CategoryId.Value, cancellationToken);
            if (categoryExists is null)
            {
                throw new NotFoundException(nameof(Category), request.CreateBudgetDto.CategoryId.Value);
            }
        }

        //var budget = _mapper.Map<Budget>(request.CreateBudgetDto);
        var budget = _mapper.Map<Budget>(request.CreateBudgetDto);
        await _budgetRepository.AddAsync(budget, cancellationToken);
        return _mapper.Map<BudgetDto>(budget);
    }
}