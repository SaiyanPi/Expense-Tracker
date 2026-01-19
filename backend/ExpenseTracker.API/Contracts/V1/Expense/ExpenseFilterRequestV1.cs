namespace ExpenseTracker.API.Contracts.V1.Expense;

public class ExpenseFilterRequestV1
{
    public DateTime? startDate {get; set;}
    public DateTime? endDate {get; set;}
    public decimal? minAmount {get; set;}
    public decimal? maxAmount {get; set;}
    public Guid? categoryId {get; set;}
    public string? userId {get; set;}

}