using System.Net;
using System.Text.Json;
using ExpenseTracker.Application.Exceptions;

namespace ExpenseTracker.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // call next middleware
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private void LogException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex,
            "Unhandled exception occurred while processing request {Method} {Path}. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message = ex.Message;

        switch (ex)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;
            case ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                break;
            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                break;
            case ForbiddenException:
                statusCode = HttpStatusCode.Forbidden;
                break;
            case ConflictException:
                statusCode = HttpStatusCode.Conflict;
                break;
        }

        var result = JsonSerializer.Serialize(new
        {
            statusCode = (int)statusCode,
            error = ex.GetType().Name,
            message,
            traceId = context.TraceIdentifier
        });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(result);
    }
    
}