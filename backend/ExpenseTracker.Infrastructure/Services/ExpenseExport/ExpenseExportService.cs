using System.Text;
using ClosedXML.Excel;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOS.Expense;

namespace ExpenseTracker.Infrastructure.Services.ExpenseExport;

public class ExpenseExportService : IExpenseExportService
{
    public byte[] ExportToCsv(IReadOnlyList<ExpenseExportDto> expenses)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Date,Title,Description,Category,Budget,Amount");

        foreach (var e in expenses)
        {
            sb.AppendLine(
                $"{e.Date:yyyy-MM-dd}," +
                $"{Escape(e.Title)}," +
                $"{Escape(e.Description)}," +
                $"{Escape(e.Category)}," +
                $"{Escape(e.Budget)}," +
                $"{e.Amount}"
            );
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    public byte[] ExportToExcel(IReadOnlyList<ExpenseExportDto> expenses)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Expenses");

        // Header
        ws.Cell(1, 1).Value = "Date";
        ws.Cell(1, 2).Value = "Title";
        ws.Cell(1, 3).Value = "Category";
        ws.Cell(1, 4).Value = "Budget";
        ws.Cell(1, 5).Value = "Amount";

        // Data
        for (int i = 0; i < expenses.Count; i++)
        {
            var row = i + 2;
            var e = expenses[i];

            ws.Cell(row, 1).Value = e.Date;
            ws.Cell(row, 2).Value = e.Title;
            ws.Cell(row, 3).Value = e.Category;
            ws.Cell(row, 4).Value = e.Budget;
            ws.Cell(row, 5).Value = e.Amount;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}