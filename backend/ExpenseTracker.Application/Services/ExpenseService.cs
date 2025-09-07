
using AutoMapper;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTrackler.Application.DTOs.Expense;

namespace ExpenseTracker.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IMapper _mapper;
    public ExpenseService(IExpenseRepository expenseRepository, IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }


    public async Task<IReadOnlyList<ExpenseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var expenses = await _expenseRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ExpenseDto>>(expenses);
    }

    public async Task<ExpenseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<ExpenseDto?>(expense);
    }

    public async Task<Guid> CreateAsync(CreateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        // business rule: title must be unique
        var exists = await _expenseRepository.ExistsByTitleAsync(dto.Title, cancellationToken);
        if (exists)
        {
            throw new ValidationException("title", $"An expense with the title '{dto.Title}' already exists.");
        }

        var expense = _mapper.Map<Expense>(dto);
        await _expenseRepository.AddAsync(expense, cancellationToken);
        return expense.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
        if (expense is null)
        {
            throw new KeyNotFoundException($"Expense with id {id} not found");
        }

        _mapper.Map(dto, expense);
        await _expenseRepository.UpdateAsync(expense, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
        if (expense is null)
        {
            throw new KeyNotFoundException($"Expense with id {id} not found");
        }

        await _expenseRepository.DeleteAsync(expense, cancellationToken);
    }
}   
