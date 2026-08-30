namespace ExpenseTracker.API.Contracts.V1.Dashboard;

public class DashboardRequestV1
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}