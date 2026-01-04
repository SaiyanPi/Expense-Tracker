using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOS.Expense;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;

public class ExportExpensesQueryHandler : IRequestHandler<ExportExpensesQuery, ExportFileResultDto>
{
    private readonly IExpenseExportService _exportService;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
     private readonly IUserRoleService _userRoleService;
    private readonly IMapper _mapper;
    public ExportExpensesQueryHandler(
        IExpenseExportService exportService,
        IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IUserRoleService userRoleService,
        IMapper mapper)
    {
        _exportService = exportService;
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;
        _userRoleService = userRoleService;
        _mapper = mapper;
    }

    public async Task<ExportFileResultDto> Handle(
        ExportExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        var isAdmin = await _userRoleService.IsAdminAsync(userId);
        
        var effectiveUserId = isAdmin ? request.Filter.UserId : userId;

        // Validate only if admin provided a userId
        if (isAdmin && !string.IsNullOrWhiteSpace(effectiveUserId))
        {
            var user = await _userRepository.GetByIdAsync(effectiveUserId);
            if (user is null)
                throw new NotFoundException(nameof(User), effectiveUserId);
        }

        if (request.Filter.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Filter.CategoryId.Value);
            if(category is null)
                throw new NotFoundException(nameof(Category), request.Filter.CategoryId.Value);
        }

        // build query
        var filter = request.Filter;

        IQueryable<Expense> query = _expenseRepository.GetExpensesQueryable();

        if (filter.StartDate.HasValue)
            query = query.Where(e => e.Date >= filter.StartDate.Value);
    
        if (filter.EndDate.HasValue)
            query = query.Where(e => e.Date <= filter.EndDate.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(e => e.Amount >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(e => e.Amount <= filter.MaxAmount.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(e => e.CategoryId == filter.CategoryId.Value);

        if (!string.IsNullOrEmpty(effectiveUserId))
            query = query.Where(e => e.UserId == effectiveUserId);

        var filteredExpenses = await query.ToListAsync(cancellationToken);

        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseExportDto>>(filteredExpenses);
        
        return request.Format.ToLower() switch
        {
            "csv" => new ExportFileResultDto
            {
                Content = _exportService.ExportToCsv(mappedExpenses),
                ContentType = "text/csv",
                FileName = $"expenses_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            },

            "excel" or "xlsx" => new ExportFileResultDto
            {
                Content = _exportService.ExportToExcel(mappedExpenses),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"expenses_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            },

            "pdf" => new ExportFileResultDto
            {
                Content = _exportService.ExportToPdf(mappedExpenses),
                ContentType = "application/pdf",
                FileName = $"expenses_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            },

            _ => throw new ValidationException("Unsupported export format")
        };
    }
}