// using System.Security.Claims;
// using ExpenseTracker.Application.Common.Interfaces.Services;
// using ExpenseTracker.Application.Common.Security;
// using ExpenseTracker.Domain.Entities;
// using ExpenseTracker.Domain.SharedKernel;

// namespace ExpenseTracker.API.Middleware;

// public class AuthorizationFailureLoggingMiddleware
// {
//     private readonly RequestDelegate _next;
//     public AuthorizationFailureLoggingMiddleware(RequestDelegate next)
//     {
//         _next = next;
//     }

//     public async Task InvokeAsync(HttpContext context)
//     {
//         // Console.WriteLine("AuthorizationFailureLoggingMiddleware invoked.");

//         // Run the rest of the pipeline first
//         await _next(context);

//         // Check if the response is 403 Forbidden
//         if (context.Response.StatusCode != StatusCodes.Status403Forbidden)
//             return;

//         // Only log if user is authenticated
//         var user = context.User;
//         if (user?.Identity?.IsAuthenticated != true)
//             return;

//         // Extract identity info
//         var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//         var userEmail = user.FindFirst(ClaimTypes.Email)?.Value;

//         // Fire-and-forget logging using a scoped service
//         _ = LogAccessDeniedAsync(context, userId, userEmail);
//     }

//     private async Task LogAccessDeniedAsync(HttpContext context, string? userId, string? userEmail)
//     { 
//         // Create a scope to safely use EF Core
//         using var scope = context.RequestServices.CreateScope();
//         var securityEventLogger = scope.ServiceProvider.GetRequiredService<ISecurityEventLogger>();

//         await securityEventLogger.LogSecurityEventAsync(new SecurityEventLog
//         {
//             EventType = SecurityEventTypes.AccessDenied,
//             UserId = userId,
//             UserEmail = userEmail,
//             Outcome = SecurityEventOutcome.Denied,
//             IpAddress = SecurityEventContext.GetIp(context),
//             Endpoint = SecurityEventContext.GetEndpoint(context),
//             CorrelationId = SecurityEventContext.GetCorrelationId(context),
//             UserAgent = SecurityEventContext.GetUserAgent(context)
//         });
//     }
// }


// // Handle 403 Forbidden responses by logging security events for auditing purposes