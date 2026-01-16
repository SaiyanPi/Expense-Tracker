using System.Security.Authentication;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Security;
using ExpenseTracker.Application.DTOs.Auth;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Features.Identity.Commands.Login;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResultDto>
{
    private readonly IIdentityService _identityService;
    private readonly ISecurityEventLogger _securityEventLogger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LoginUserCommandHandler(
        IIdentityService identityService,
        ISecurityEventLogger securityEventLogger,
        IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _securityEventLogger = securityEventLogger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
      
        var result = await _identityService.LoginAsync(request.LoginUserDto, cancellationToken);

        if (!result.Success)
        {
            // Log failed login event
            await _securityEventLogger.LogSecurityEventAsync(new SecurityEventLog
            {
                EventType = SecurityEventTypes.LoginFailed,
                UserId = null,
                UserEmail = request.LoginUserDto.Email,
                CorrelationId = SecurityEventContext.GetCorrelationId(httpContext!),
                IpAddress = SecurityEventContext.GetIp(httpContext!),
                Endpoint = SecurityEventContext.GetEndpoint(httpContext!),
                Outcome = SecurityEventOutcome.Failed,
                UserAgent = SecurityEventContext.GetUserAgent(httpContext!)
            });

            throw new InvalidCredentialsException(result.Errors != null ? string.Join("; ", result.Errors) : "Login failed.");
        }

        // Log successful login event
        await _securityEventLogger.LogSecurityEventAsync(new SecurityEventLog
        {
            EventType = SecurityEventTypes.LoginSuccess,
            UserId = null, // AuthResultDto does not contain UserId(by design)
            UserEmail = request.LoginUserDto.Email,
            CorrelationId = SecurityEventContext.GetCorrelationId(httpContext!),
            IpAddress = SecurityEventContext.GetIp(httpContext!),
            Endpoint = SecurityEventContext.GetEndpoint(httpContext!),
            Outcome = SecurityEventOutcome.Success,
            UserAgent = SecurityEventContext.GetUserAgent(httpContext!)
        });

        // log token issued event
        await _securityEventLogger.LogSecurityEventAsync(new SecurityEventLog
        {
            EventType = SecurityEventTypes.TokenIssued,
            UserId = null,
            UserEmail = request.LoginUserDto.Email,
            CorrelationId = SecurityEventContext.GetCorrelationId(httpContext!),
            IpAddress = SecurityEventContext.GetIp(httpContext!),
            Endpoint = SecurityEventContext.GetEndpoint(httpContext!),
            Outcome = SecurityEventOutcome.Success,
            UserAgent = SecurityEventContext.GetUserAgent(httpContext!)
        });

        return result;
    }
}