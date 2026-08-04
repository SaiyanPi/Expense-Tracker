namespace ExpenseTracker.Application.Common.Interfaces.Services;
public interface ICacheVersionService
{
    int GetVersion(string cacheGroup, string userId);

    void IncrementVersion(string cacheGroup, string userId);
}