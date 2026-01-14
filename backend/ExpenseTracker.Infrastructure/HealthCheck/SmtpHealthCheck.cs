using System.Net.Sockets;
using ExpenseTracker.Infrastructure.Services.Email;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ExpenseTracker.Infrastructure.HealthCheck;

public class SmtpHealthCheck : IHealthCheck
{
    private readonly SmtpSettings _smtpSettings;

    public SmtpHealthCheck(SmtpSettings smtpSettings)
    {
        _smtpSettings = smtpSettings;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var tcpClient = new TcpClient();
            var connectTask = tcpClient.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port);

            if (await Task.WhenAny(connectTask, Task.Delay(TimeSpan.FromSeconds(5), cancellationToken)) == connectTask)
            {
                await connectTask; // Ensure the connection completed successfully
                return HealthCheckResult.Healthy("SMTP server is reachable");
            }
            else
            {
                return HealthCheckResult.Unhealthy("SMTP server connection timeout");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"SMTP server is not reachable: {ex.Message}");
        }
    }
}
