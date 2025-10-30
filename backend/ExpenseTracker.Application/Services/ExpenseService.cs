using AutoMapper;
using ExpenseTracker.Application.Exceptions;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTrackler.Application.DTOs.Expense;
using Microsoft.AspNetCore.Http.HttpResults;

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
        if (expense is null)
            throw new NotFoundException(nameof(Expense), id);

        return _mapper.Map<ExpenseDto?>(expense);
    }

    public async Task<Guid> CreateAsync(CreateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        // business rule: title must be unique
        var exists = await _expenseRepository.ExistsByTitleAsync(dto.Title, cancellationToken);
        if (exists)
        {
            throw new ConflictException($"Expense with title '{dto.Title}' already exists.");
        }

        var expense = _mapper.Map<Expense>(dto);
        await _expenseRepository.AddAsync(expense, cancellationToken);
        return expense.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        // check if expense exists
        var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
        if (expense is null)
            throw new NotFoundException(nameof(Expense), id);
            
        // business rule: title must be unique
        var exists = await _expenseRepository.ExistsByTitleAsync(dto.Title, cancellationToken);
        if (exists && !string.Equals(expense.Title, dto.Title, StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException($"Expense with title '{dto.Title}' already exists.");
        }

        _mapper.Map(dto, expense);
        
        // save changes
        try
        {
            await _expenseRepository.UpdateAsync(expense, cancellationToken);
        }
         catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedException("You are not authorized to update this expense.");
        }
        catch (Exception ex)
        {
            // Optionally rethrow as ValidationException for consistency
            throw new ValidationException($"Failed to update expense: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, cancellationToken);
        if (expense is null)
            throw new NotFoundException(nameof(Expense), id);

        await _expenseRepository.DeleteAsync(expense, cancellationToken);
    }
}   
