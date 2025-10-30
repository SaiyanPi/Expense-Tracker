using System.Net;
using System.Text.Json;
using Azure;
using ExpenseTracker.API.Models;
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
            await _next(context); 
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    // for logging purposes
    private void LogException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex,
            "Unhandled exception occurred while processing request {Method} {Path}. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Log all exceptions (optional: could skip known handled ones)
        LogException(context, ex);

        var response = new ErrorResponse
        {
            Error = ex.GetType().Name,
            Message = ex.Message,
            TraceId = context.TraceIdentifier
        };
       


        // switch expression for concise status mapping
        var statusCode = ex switch
        {
            FluentValidation.ValidationException => HttpStatusCode.BadRequest,
            ValidationException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            ConflictException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };

        // Customize FluentValidation error details
        if (ex is FluentValidation.ValidationException fvEx)
        {
            response.Message = "One or more validation errors occurred.";
            response.Details = fvEx.Errors.Select(e => new
            {
                e.PropertyName,
                e.ErrorMessage
            });
        }
        
        response.StatusCode = (int)statusCode;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
    
}