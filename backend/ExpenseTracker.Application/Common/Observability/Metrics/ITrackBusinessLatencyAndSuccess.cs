namespace ExpenseTracker.Application.Common.Observability.Metrics;

public interface ITrackBusinessLatencyAndSuccess
{
    string OperationName { get; }
}