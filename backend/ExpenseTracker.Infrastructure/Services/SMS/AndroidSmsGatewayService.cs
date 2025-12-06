using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using ExpenseTracker.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace ExpenseTracker.Infrastructure.Services.SMS;

public class AndroidSmsGatewayService : ISmsSenderService
{
    private readonly HttpClient _httpClient;
    private readonly string _gatewayUrl;
    private readonly string? _authHeader; // if username/password required

    public AndroidSmsGatewayService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _gatewayUrl = config["SmsGateway:BaseUrl"] ?? throw new ArgumentNullException("SmsGateway:BaseUrl", "SmsGateway:BaseUrl configuration value is missing."); 
        var user = config["SmsGateway:Username"];
        var pass = config["SmsGateway:Password"];
        if (!string.IsNullOrEmpty(user))
        {
            var creds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}"));
            _authHeader = $"Basic {creds}";
        }
    }

    public async Task SendOtpAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(toPhoneNumber))
            throw new ArgumentException("Destinstion phone number cannot be empty.");

        // Format to E.164 if missing '+' and country code
        string formattedToPhoneNumber = FormatToE164(toPhoneNumber);

        var request = new {
            textMessage = new { text = $"Your OTP is: {message}" },
            phoneNumbers = new [] { formattedToPhoneNumber }
        };
        var reqMsg = new HttpRequestMessage(HttpMethod.Post, _gatewayUrl)
        {
            Content = JsonContent.Create(request)
        };
        if (_authHeader != null)
            reqMsg.Headers.Authorization = AuthenticationHeaderValue.Parse(_authHeader);

        await _httpClient.SendAsync(reqMsg);
    }

    private string FormatToE164(string toPhoneNumber)
    {
        // if already starts with '+', assume E.164
        if (toPhoneNumber.StartsWith("+")) return toPhoneNumber;

        // we can customize default country code (Nepal: +977)
        return $"+977{toPhoneNumber}";
    }
}
