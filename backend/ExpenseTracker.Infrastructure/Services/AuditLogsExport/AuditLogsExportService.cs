using System.Text;
using ClosedXML.Excel;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.AuditLog;
using ExpenseTracker.Application.DTOS.Expense;
using Microsoft.AspNetCore.Authentication;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace ExpenseTracker.Infrastructure.Services.AuditLogsExport;

public class AuditLogsExportService : IAuditLogsExportService
{
    public byte[] ExportToCsv(IReadOnlyList<AuditLogsExportDto> auditLogs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CreatedAt, EntityName, EntityId, Action, OldValues, NewValues, UserId, CorrelationId, HttpMethod, RequestPath, ClientIp, UserAgent");

        foreach (var a in auditLogs)
        {
            sb.AppendLine(
                $"{a.CreatedAt:yyyy-MM-dd}," +
                $"{Escape(a.EntityName.ToString())}," +
                $"{Escape(a.EntityId.ToString())}," +
                $"{Escape(a.Action.ToString())}," +
                $"{Escape(a.OldValues ?? string.Empty)}," +
                $"{Escape(a.NewValues ?? string.Empty)}," +
                $"{Escape(a.UserId ?? string.Empty)}," +
               
                $"{Escape(a.CorrelationId)}," +
                $"{Escape(a.HttpMethod ?? string.Empty)}," +
                $"{Escape(a.RequestPath ?? string.Empty)}," +
                $"{Escape(a.ClientIp ?? string.Empty)}," +
                $"{a.UserAgent}"
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

    public byte[] ExportToExcel(IReadOnlyList<AuditLogsExportDto> auditLogs)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Expenses");

        // Header
        ws.Cell(1, 1).Value = "Date";
        ws.Cell(1, 2).Value = "EntityName";
        ws.Cell(1, 3).Value = "EntityId";
        ws.Cell(1, 4).Value = "Action";
        ws.Cell(1, 5).Value = "OldValues";
        ws.Cell(1, 6).Value = "NewValues";
        ws.Cell(1, 7).Value = "UserId";
        ws.Cell(1, 8).Value = "CorrelationId";
        ws.Cell(1, 9).Value = "HttpMethod";
        ws.Cell(1, 10).Value = "RequestPath";
        ws.Cell(1, 11).Value = "ClientIp";
        ws.Cell(1, 12).Value = "UserAgent";

        // Data
        for (int i = 0; i < auditLogs.Count; i++)
        {
            var row = i + 2;
            var a = auditLogs[i];

            ws.Cell(row, 1).Value = a.CreatedAt;
            ws.Cell(row, 2).Value = a.EntityName.ToString();
            ws.Cell(row, 3).Value = a.EntityId.ToString();
            ws.Cell(row, 4).Value = a.Action.ToString();
            ws.Cell(row, 5).Value = a.OldValues;
            ws.Cell(row, 6).Value = a.NewValues;
            ws.Cell(row, 7).Value = a.UserId;
            ws.Cell(row, 8).Value = a.CorrelationId;
            ws.Cell(row, 9).Value = a.HttpMethod;
            ws.Cell(row, 10).Value = a.RequestPath;
            ws.Cell(row, 11).Value = a.ClientIp;
            ws.Cell(row, 12).Value = a.UserAgent;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // public byte[] ExportToPdf(IReadOnlyList<AuditLogsExportDto> auditLogs)
    // {
    //     return Document.Create(container =>
    //     {
    //         container.Page(page =>
    //         {
    //             page.Size(PageSizes.A4);
    //             page.Margin(10);
    //             page.DefaultTextStyle(x => x.FontSize(10));

    //             // Header
    //             page.Header()
    //                 .Text("Audit Logs")
    //                 .FontSize(20)
    //                 .Bold()
    //                 .AlignCenter();

    //             // Content
    //             page.Content().PaddingTop(10).Table(table =>
    //             {
    //                 table.ColumnsDefinition(columns =>
    //                 {
    //                     columns.RelativeColumn(2); // CreatedAt
    //                     columns.RelativeColumn(2); // EntityName
    //                     columns.RelativeColumn(3); // EntityId
    //                     columns.RelativeColumn(2); // Action
    //                     columns.RelativeColumn(4); // OldValues
    //                     columns.RelativeColumn(4); // NewValues
    //                     columns.RelativeColumn(3); // UserId
    //                     columns.RelativeColumn(3); // CorrelationId
    //                     columns.RelativeColumn(2); // HttpMethod
    //                     columns.RelativeColumn(2); // RequestPath
    //                     columns.RelativeColumn(2); // ClientIp
    //                     columns.RelativeColumn(2); // UserAgent
    //                 });

    //                 // Table header
    //                 table.Header(header =>
    //                 {
    //                     header.Cell().Text("CreatedAt").Bold();
    //                     header.Cell().Text("EntityName").Bold();
    //                     header.Cell().Text("EntityId").Bold();
    //                     header.Cell().Text("Action").Bold();
    //                     header.Cell().Text("OldValues").Bold();
    //                     header.Cell().Text("NewValues").Bold();
    //                     header.Cell().Text("UserId").Bold();
    //                     header.Cell().Text("CorrelationId").Bold();
    //                     header.Cell().Text("HttpMethod").Bold();
    //                     header.Cell().Text("RequestPath").Bold();
    //                     header.Cell().Text("ClientIp").Bold();
    //                     header.Cell().Text("UserAgent").Bold();
    //                 });

    //                 // Table rows
    //                 foreach (var a in auditLogs)
    //                 {
    //                     table.Cell().Text(a.CreatedAt.ToString("yyyy-MM-dd"));
    //                     table.Cell().Text(a.EntityName);
    //                     table.Cell().Text(a.EntityId);
    //                     table.Cell().Text(a.Action.ToString());
    //                     table.Cell().Text(a.OldValues);
    //                     table.Cell().Text(a.NewValues);
    //                     table.Cell().Text(a.UserId);
    //                     table.Cell().Text(a.CorrelationId);
    //                     table.Cell().Text(a.HttpMethod);
    //                     table.Cell().Text(a.RequestPath);
    //                     table.Cell().Text(a.ClientIp);
    //                     table.Cell().Text(a.UserAgent);
    //                 }
    //             });

    //             // Footer
    //             page.Footer()
    //                 .AlignRight()
    //                 .Text($"Generated on {DateTime.UtcNow.ToLocalTime():yyyy-MM-dd HH:mm}");

    //         });
    //     })
    //     .GeneratePdf();
    // }

    public byte[] ExportToPdf(IReadOnlyList<AuditLogsExportDto> auditLogs)
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
                    .Text("Audit Logs Report")
                    .FontSize(18)
                    .Bold()
                    .AlignCenter();

                page.Content().PaddingTop(10).Column(column =>
                {
                    foreach (var a in auditLogs)
                    {
                        // Each audit log gets its own table (2-column layout)
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

                            AddRow("Created At", a.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
                            AddRow("Entity", $"{a.EntityName} ({a.EntityId})");
                            AddRow("Action", a.Action.ToString());
                            AddRow("User Id", a.UserId);
                            AddRow("Correlation Id", a.CorrelationId);
                            AddRow("HTTP Method", a.HttpMethod);
                            AddRow("Request Path", a.RequestPath);
                            AddRow("Client IP", a.ClientIp);
                            AddRow("User Agent", a.UserAgent);

                            // Wrap JSON values in separate rows for readability
                            AddRow("Old Values", a.OldValues);
                            AddRow("New Values", a.NewValues);
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