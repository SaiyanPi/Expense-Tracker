namespace ExpenseTracker.API.Contracts.V1.Expense
{
    public class ExpenseExportResponseV1
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Category { get; set; } = default!;
        public string Budget { get; set; } = default!;
        public DateTime Date { get; set; }
    }
}