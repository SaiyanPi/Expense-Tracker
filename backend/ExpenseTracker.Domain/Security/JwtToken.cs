namespace ExpenseTracker.Domain.Security;

public record JwtToken(
    string AccessToken,
    DateTime ExpiresAtUtc
);