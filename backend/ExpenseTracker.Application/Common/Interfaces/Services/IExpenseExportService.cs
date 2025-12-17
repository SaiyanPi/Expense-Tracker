using ExpenseTracker.Application.DTOS.Expense;

namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IExpenseExportService
{
    byte[] ExportToCsv(IReadOnlyList<ExpenseExportDto> expenses);
    byte[] ExportToExcel(IReadOnlyList<ExpenseExportDto> expenses);
}
