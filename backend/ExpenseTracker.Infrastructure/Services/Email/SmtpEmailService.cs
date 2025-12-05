using System.Net;
using System.Net.Mail;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;

namespace ExpenseTracker.Infrastructure.Services.Email;

public class SmtpEmailService : IEmailService
{
   private readonly SmtpSettings _smtpSettings;
   public SmtpEmailService(SmtpSettings smtpSettings)
   {
       _smtpSettings = smtpSettings;
   }

   public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
   {
        try
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EmailSendingException("SMTP service failed to send email.", ex);
        }
   }
   
}