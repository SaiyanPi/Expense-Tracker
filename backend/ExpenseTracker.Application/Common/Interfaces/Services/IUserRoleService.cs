namespace ExpenseTracker.Application.Common.Interfaces.Services;

public interface IUserRoleService
{
    Task<bool> IsAdminAsync(string userId);
}