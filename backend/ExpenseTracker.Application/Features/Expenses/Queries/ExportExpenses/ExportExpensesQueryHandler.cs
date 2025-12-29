using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOS.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.ExportExpenses;

public class ExportExpensesQueryHandler : IRequestHandler<ExportExpensesQuery, ExportFileResultDto>
{
    private readonly IExpenseExportService _exportService;
    private readonly IExpenseRepository _repository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;
    public ExportExpensesQueryHandler(
        IExpenseExportService exportService,
        IExpenseRepository repository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _exportService = exportService;
        _repository = repository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<ExportFileResultDto> Handle(
        ExportExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var expenses = await _repository.GetExpensesForExportAsync(
            userId,
            request.startDate,
            request.endDate,
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

            "pdf" => new ExportFileResultDto
            {
                Content = _exportService.ExportToPdf(mappedExpenses),
                ContentType = "application/pdf",
                FileName = "expenses.pdf"
            },

            _ => throw new ValidationException("Unsupported export format")
        };
    }
}