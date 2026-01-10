using DocumentFormat.OpenXml.Office2013.Excel;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Domain.SharedKernel;

namespace ExpenseTracker.Infrastructure.Repositories;
public class EntityResolverRepository : IEntityResolverRepository
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly IExpenseRepository _expenseRepository;

    public EntityResolverRepository(
        ICategoryRepository categoryRepository,
        IBudgetRepository budgetRepository,
        IExpenseRepository expenseRepository)
    {
        _categoryRepository = categoryRepository;
        _budgetRepository = budgetRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<BaseEntity?> ExistsAsync(EntityType entityName, Guid entityId, CancellationToken cancellationToken = default)
    {
        return entityName switch
        {
            EntityType.Category => await _categoryRepository.GetByIdAsync(entityId, cancellationToken),
            EntityType.Budget => await _budgetRepository.GetByIdAsync(entityId, cancellationToken),
            EntityType.Expense => await _expenseRepository.GetByIdAsync(entityId, cancellationToken),
            _ => null
        };
    }
}
