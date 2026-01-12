namespace ExpenseTracker.Application.Common.Observability.Metrics;

public interface ITrackBusinessLatency
{
    string OperationName { get; }
}