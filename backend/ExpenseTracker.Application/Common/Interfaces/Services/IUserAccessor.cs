namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IUserAccessor
{
    string UserId { get; }
    string UserEmail { get; }
}