namespace ExpenseTracker.Application.Common.Interfaces.Services;


public interface IProfileImageStorageService
{
    Task<string> SaveAsync(Stream image, string fileName, CancellationToken cancellationToken = default);

    Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default);
}