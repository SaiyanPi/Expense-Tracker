using System.Text;
using ClosedXML.Excel;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Application.DTOs.SecurityEventLog;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace ExpenseTracker.Infrastructure.Services.SecurityEventLogsExport;

public class SecurityEventLogsExportService : ISecurityEventLogsExportService
{
    public byte[] ExportToCsv(IReadOnlyList<SecurityLogsExportDto> securityLogs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Timestamp, UserId, UserEmail, EventType, Outcome, CorrelationId, IpAddress, Endpoint, UserAgent");

        foreach (var s in securityLogs)
        {
            sb.AppendLine(
                $"{s.Timestamp:yyyy-MM-dd}," +
                $"{Escape(s.UserId ?? string.Empty)}," +
                $"{Escape(s.UserEmail ?? string.Empty)}," +
                $"{Escape(s.EventType.ToString())}," +
                $"{Escape(s.Outcome.ToString())}," +
                $"{Escape(s.CorrelationId ?? string.Empty)}," +
                $"{Escape(s.IpAddress ?? string.Empty)}," +
                $"{Escape(s.Endpoint ?? string.Empty)}," +             
                $"{s.UserAgent}"
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

    public byte[] ExportToExcel(IReadOnlyList<SecurityLogsExportDto> securityLogs)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Security Event Logs");

        // Header
        ws.Cell(1, 1).Value = "Timestamp";
        ws.Cell(1, 2).Value = "UserId";
        ws.Cell(1, 3).Value = "UserEmail";
        ws.Cell(1, 4).Value = "EventType";
        ws.Cell(1, 5).Value = "Outcome";
        ws.Cell(1, 6).Value = "CorrelationId";
        ws.Cell(1, 7).Value = "IpAddress";
        ws.Cell(1, 8).Value = "Endpoint";
        ws.Cell(1, 9).Value = "UserAgent";

        // Data
        for (int i = 0; i < securityLogs.Count; i++)
        {
            var row = i + 2;
            var a = securityLogs[i];

            ws.Cell(row, 1).Value = a.Timestamp;
            ws.Cell(row, 2).Value = a.UserId;
            ws.Cell(row, 3).Value = a.UserEmail;
            ws.Cell(row, 4).Value = a.EventType.ToString();
            ws.Cell(row, 5).Value = a.Outcome.ToString();
            ws.Cell(row, 6).Value = a.CorrelationId;
            ws.Cell(row, 7).Value = a.IpAddress;
            ws.Cell(row, 8).Value = a.Endpoint;
            ws.Cell(row, 9).Value = a.UserAgent;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportToPdf(IReadOnlyList<SecurityLogsExportDto> securityLogs)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10));

                // Header
                page.Header()
                    .Text("Security Event Logs Report")
                    .FontSize(18)
                    .Bold()
                    .AlignCenter();

                page.Content().PaddingTop(10).Column(column =>
                {
                    foreach (var s in securityLogs)
                    {
                        // Each security log gets its own table (2-column layout)
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(120); // Label
                                columns.RelativeColumn();    // Value
                            });

                            void AddRow(string label, string? value)
                            {
                                table.Cell().Element(c => c.PaddingBottom(2).Text(label).Bold());
                                table.Cell().Element(c => c.PaddingBottom(2).Text(value ?? "-"));
                            }

                            AddRow("Timestamp", s.Timestamp.ToString("yyyy-MM-dd HH:mm"));
                            AddRow("User Id", s.UserId);
                            AddRow("User Email", s.UserEmail);
                            AddRow("Event Type", s.EventType.ToString());
                            AddRow("Outcome", s.Outcome.ToString());
                            AddRow("Correlation Id", s.CorrelationId);
                            AddRow("Ip Address", s.IpAddress);
                            AddRow("Endpoint", s.Endpoint);
                            AddRow("User Agent", s.UserAgent);
                        });

                        // Spacer between audit logs
                        column.Item().PaddingVertical(5).LineHorizontal(1);
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