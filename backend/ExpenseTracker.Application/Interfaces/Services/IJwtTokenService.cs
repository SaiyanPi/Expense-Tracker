using System.Security.Claims;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Interfaces.Services;

public interface IJwtTokenService
{
    // Generates a new JWT token for a user
    (string Token, DateTime ExpiresAt) GenerateToken(User user, IList<string> roles);

    // Generates a refresh token
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();

    // Validates expired JWT and returns claims principal
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}