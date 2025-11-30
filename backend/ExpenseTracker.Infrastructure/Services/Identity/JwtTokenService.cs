// using ExpenseTracker.Application.Common.Interfaces.Services;
// using ExpenseTracker.Domain.Entities;
// using Microsoft.Extensions.Configuration;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Security.Cryptography;
// using System.Text;

// namespace ExpenseTracker.Infrastructure.Services.Identity;

// public class JwtTokenService : IJwtTokenService
// {
//     private readonly IConfiguration _configuration;

//     public JwtTokenService(IConfiguration configuration)
//     {
//         _configuration = configuration;
//     }

//     public (string Token, DateTime ExpiresAt) GenerateToken(User user, IList<string> roles)
//     {
//         var expiresAt = DateTime.UtcNow.AddHours(1);
//         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"]!));
//         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//         var claims = new List<Claim>
//         {
//             new(ClaimTypes.NameIdentifier, user.Id),
//             new(ClaimTypes.Name, user.FullName),
//             new(ClaimTypes.Email, user.Email ?? string.Empty)
//         };

//         foreach (var role in roles)
//         {
//             claims.Add(new Claim(ClaimTypes.Role, role));
//         }

//         var token = new JwtSecurityToken(
//             issuer: _configuration["JwtConfig:Issuer"],
//             audience: _configuration["JwtConfig:Audience"],
//             claims: claims,
//             expires: expiresAt,
//             signingCredentials: creds
//         );

//         return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
//     }

//     public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
//     {
//         var randomBytes = new byte[64];
//         using var rng = RandomNumberGenerator.Create();
//         rng.GetBytes(randomBytes);
//         var refreshToken = Convert.ToBase64String(randomBytes);

//         return (refreshToken, DateTime.UtcNow.AddDays(7));
//     }

//     public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
//     {
//         var tokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateAudience = false,
//             ValidateIssuer = false,
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"]!)
//             ),
//             ValidateLifetime = false // allow expired tokens
//         };

//         var tokenHandler = new JwtSecurityTokenHandler();
//         try
//         {
//             var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
//             return securityToken is JwtSecurityToken ? principal : null;
//         }
//         catch
//         {
//             return null;
//         }
//     }
// }
