namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface ISmsSenderService
{
    Task SendOtpAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}