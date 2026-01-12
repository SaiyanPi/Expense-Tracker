using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Azure;
using ClosedXML;
using ExpenseTracker.API.Models;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Observability.Metrics.BusinessMetrics.Generic;
using Serilog.Context;

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
            // Hook the failure and exception metric
            var method = context.Request.Method;
            var path = context.Request.Path; 
            var operation = $"{method} {path}"; // or extract a more business-specific operation
            BusinessFailureMetric.RecordFailure(operation, ex.GetType().Name);

            await HandleExceptionAsync(context, ex);
        }
    }

    // for logging purposes
    private void LogException(HttpContext context, Exception ex)
    {
        // using CorrelationId from source of truth(CorrelationIdMiddleware) instead of TraceIdentifier
        var correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString()
            ?? context.TraceIdentifier;
        
        // enrich logs with userId
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var logLevel = ex switch
        {
            ValidationException => LogLevel.Warning,
            NotFoundException => LogLevel.Information,
            UnauthorizedException => LogLevel.Warning,
            ForbiddenException => LogLevel.Warning,
            ConflictException => LogLevel.Warning,
            _ => LogLevel.Error
        };

        // Push into Serilog context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("UserId", userId))
        {
            _logger.Log(logLevel, ex,
                "Exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }
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
        //context.Response.Headers["Trace-Id"] = context.TraceIdentifier;
        var correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString()
            ?? context.TraceIdentifier;
        response.CorrelationId = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
    
}