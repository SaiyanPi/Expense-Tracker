using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using ExpenseTracker.Application.Common.Interfaces.Services;

namespace ExpenseTracker.Infrastructure.Services.SMS;

public class TwilioSmsSenderService : ISmsSenderService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromPhone;

    public TwilioSmsSenderService(IConfiguration configuration)
    {
        _accountSid = configuration["Twilio:AccountSid"] ?? throw new ArgumentException("Twilio AccountSid is not configured.");
        _authToken = configuration["Twilio:AuthToken"] ?? throw new ArgumentException("Twilio AuthToken is not configured.");
        _fromPhone = configuration["Twilio:FromPhone"] ?? throw new ArgumentException("Twilio FromPhone is not configured.");

        if (string.IsNullOrWhiteSpace(_accountSid))
            throw new ArgumentException("Twilio AccountSid is not configured.");
        if (string.IsNullOrWhiteSpace(_authToken))
            throw new ArgumentException("Twilio AuthToken is not configured.");
        if (string.IsNullOrWhiteSpace(_fromPhone))
            throw new ArgumentException("Twilio FromPhone is not configured.");

        TwilioClient.Init(_accountSid, _authToken);
    }

    public async Task SendOtpAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(toPhoneNumber))
            throw new ArgumentException("Destination phone number cannot be empty.");

        // Format to E.164 if missing '+' and country code
        string formattedTo = FormatToE164(toPhoneNumber);

        var from = new PhoneNumber(_fromPhone);
        var to = new PhoneNumber(formattedTo);  

        var messageResult = await MessageResource.CreateAsync(
            body: message,
            from: from,
            to: to
        );

        // Optional: log message SID for debugging
        // Console.WriteLine($"Twilio SMS sent. SID: {messageResult.Sid}");
    }

    private string FormatToE164(string phone)
    {
        // if already starts with '+', assume E.164
        if (phone.StartsWith("+")) return phone;

        // we can customize default country code (Nepal: +977)
        return $"+977{phone.TrimStart('0')}";
    }
    
}
