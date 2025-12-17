using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOS.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;

public class ExportExpensesQueryHandler : IRequestHandler<ExportExpensesQuery, ExportFileResultDto>
{
    private readonly IExpenseExportService _exportService;
    private readonly IExpenseRepository _repository;
    private readonly IMapper _mapper;
    public ExportExpensesQueryHandler(
        IExpenseExportService exportService,
        IExpenseRepository repository,
        IMapper mapper)
    {
        _exportService = exportService;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ExportFileResultDto> Handle(
        ExportExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var expenses = await _repository.GetExpensesForExportAsync(
            request.UserId,
            cancellationToken);

        var mappedExpenses = _mapper.Map<IReadOnlyList<ExpenseExportDto>>(expenses);
        
        return request.Format.ToLower() switch
        {
            "csv" => new ExportFileResultDto
            {
                Content = _exportService.ExportToCsv(mappedExpenses),
                ContentType = "text/csv",
                FileName = "expenses.csv"
            },

            "excel" or "xlsx" => new ExportFileResultDto
            {
                Content = _exportService.ExportToExcel(mappedExpenses),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = "expenses.xlsx"
            },

            _ => throw new ValidationException("Unsupported export format")
        };
    }
}