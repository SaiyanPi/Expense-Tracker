namespace ExpenseTracker.API.Contracts.V1.Expense;

public class ExportExpensesRequestV1
{
    public string format {get; set;} = string.Empty; // "csv" or "pdf"
    public DateTime? startDate {get; set;}
    public DateTime? endDate {get; set;}
    public decimal? minAmount {get; set;}
    public decimal? maxAmount {get; set;}
    public Guid? categoryId {get; set;}
    public string? userId {get; set;}
}