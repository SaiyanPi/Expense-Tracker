using System.Net;
using System.Text.Json;
using Azure;
using ExpenseTracker.API.Models;
using ExpenseTracker.Application.Common.Exceptions;

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

    public async Task InvokeAsync(HttpContext context)
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
        var logLevel = ex switch
        {
            ValidationException => LogLevel.Warning,
            NotFoundException => LogLevel.Information,
            UnauthorizedException => LogLevel.Warning,
            ForbiddenException => LogLevel.Warning,
            ConflictException => LogLevel.Warning,
            _ => LogLevel.Error
        };

        _logger.Log(logLevel, ex,
            "Exception while processing {Method} {Path}. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);
    }


    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        if (ex is AggregateException aggEx)
            ex = aggEx.Flatten().InnerExceptions.FirstOrDefault() ?? ex;
        // Log all exceptions (optional: could skip known handled ones)
        LogException(context, ex);


        // switch expression for concise status mapping
        var statusCode = ex switch
        {
            FluentValidation.ValidationException => HttpStatusCode.BadRequest,
            ValidationException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            BadRequestException => HttpStatusCode.BadRequest,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            ConflictException => HttpStatusCode.Conflict,
            IdentityOperationException => HttpStatusCode.BadRequest, 
            DomainException => HttpStatusCode.BadRequest,
            InvalidCredentialsException => HttpStatusCode.Unauthorized,  
            Application.Common.Exceptions.InvalidOperationException => HttpStatusCode.BadRequest,
            EmailSendingException => HttpStatusCode.ServiceUnavailable,
            _ => HttpStatusCode.InternalServerError
        };

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Error = ex.GetType().Name,
            Message = statusCode == HttpStatusCode.InternalServerError
                ? "An unexpected error occurred. Please try again later."
                : ex.Message,
            TraceId = context.TraceIdentifier
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
        context.Response.Headers["Trace-Id"] = context.TraceIdentifier;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
    
}