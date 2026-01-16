namespace ExpenseTracker.Domain.SharedKernel;

public enum SecurityEventTypes
{
    LoginSuccess = 1,
    LoginFailed = 2,
    TokenIssued = 3,
    Logout = 4,
    AccessDenied = 5,  // 403 forbidden
    UnauthorizedAccess = 6  // 401 unauthorized
}