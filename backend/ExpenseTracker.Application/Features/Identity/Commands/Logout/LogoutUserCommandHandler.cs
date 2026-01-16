using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Security;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Features.Identity.Commands.Logout;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Unit>
{
    private readonly IIdentityService _identityService;
    private readonly IUserAccessor _userAccessor;
    private readonly ISecurityEventLogger _securityEventLogger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogoutUserCommandHandler(
        IIdentityService identityService,
        IUserAccessor userAccessor,
        ISecurityEventLogger securityEventLogger,
        IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _userAccessor = userAccessor;
        _securityEventLogger = securityEventLogger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Unit> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext!;
        var userId = _userAccessor.UserId;
        var userEmail = _userAccessor.UserEmail;

        await _identityService.LogoutAsync(userId, cancellationToken);

        await _securityEventLogger.LogSecurityEventAsync(new SecurityEventLog
        {
            EventType = SecurityEventTypes.Logout,
            UserId = userId,
            UserEmail = userEmail,
            CorrelationId = SecurityEventContext.GetCorrelationId(context),
            IpAddress = SecurityEventContext.GetIp(context),
            Endpoint = SecurityEventContext.GetEndpoint(context),
            Outcome = SecurityEventOutcome.Success,
            UserAgent = SecurityEventContext.GetUserAgent(context)
        });

        return Unit.Value;
    }
}