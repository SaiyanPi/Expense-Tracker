using System.Text;
using ClosedXML.Excel;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOS.Expense;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

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
        ws.Cell(1, 3).Value = "Description";
        ws.Cell(1, 4).Value = "Category";
        ws.Cell(1, 5).Value = "Budget";
        ws.Cell(1, 6).Value = "Amount";

        // Data
        for (int i = 0; i < expenses.Count; i++)
        {
            var row = i + 2;
            var e = expenses[i];

            ws.Cell(row, 1).Value = e.Date;
            ws.Cell(row, 2).Value = e.Title;
            ws.Cell(row, 3).Value = e.Description;
            ws.Cell(row, 4).Value = e.Category;
            ws.Cell(row, 5).Value = e.Budget;
            ws.Cell(row, 6).Value = e.Amount;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportToPdf(IReadOnlyList<ExpenseExportDto> expenses)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(11));

                // Header
                page.Header()
                    .Text("Expense Report")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                // Content
                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Date
                        columns.RelativeColumn(2); // Title
                        columns.RelativeColumn(3); // Description
                        columns.RelativeColumn(3); // Category
                        columns.RelativeColumn(3); // Budget
                        columns.RelativeColumn(2); // Amount
                    });

                    // Table header
                    table.Header(header =>
                    {
                        header.Cell().Text("Date").Bold();
                        header.Cell().Text("Title").Bold();
                        header.Cell().Text("Description").Bold();
                        header.Cell().Text("Category").Bold();
                        header.Cell().Text("Budget").Bold();
                        header.Cell().Text("Amount").Bold();
                    });

                    // Table rows
                    foreach (var e in expenses)
                    {
                        table.Cell().Text(e.Date.ToString("yyyy-MM-dd"));
                        table.Cell().Text(e.Title);
                        table.Cell().Text(e.Description);
                        table.Cell().Text(e.Category);
                        table.Cell().Text(e.Budget);
                        table.Cell().Text(e.Amount.ToString("N2"));
                    }
                });

                // Footer
                page.Footer()
                    .AlignRight()
                    .Text($"Generated on {DateTime.UtcNow.ToLocalTime():yyyy-MM-dd HH:mm}");

            });
        })
        .GeneratePdf();
    }
}