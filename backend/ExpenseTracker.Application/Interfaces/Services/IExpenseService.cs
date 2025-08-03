using ExpenseTrackler.Application.DTOs.Expense;

namespace ExpenseTracker.Application.Interfaces.Services;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetAllAsync();
    Task<ExpenseDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateExpenseDto dto);
    Task UpdateAsync(UpdateExpenseDto dto);
    Task DeleteAsync(Guid id);
}

// The application/service layer orchestrates changes