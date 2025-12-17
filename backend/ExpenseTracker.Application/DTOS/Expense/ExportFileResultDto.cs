namespace ExpenseTracker.Application.DTOS.Expense;

public class ExportFileResultDto
{
    public byte[] Content { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public string FileName { get; init; } = default!;
}