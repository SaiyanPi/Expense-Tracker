namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface ISmsSenderService
{
    Task SendOtpAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default);
}