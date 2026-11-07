using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Application.Common.Options;

namespace Portfolio.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SmtpHost))
        {
            _logger.LogWarning(
                "SMTP host is not configured (EmailSettings:SmtpHost). Message to {To} with subject '{Subject}' was logged but NOT sent.",
                toEmail, subject);
            return;
        }

        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, string.IsNullOrWhiteSpace(_settings.SenderName) ? _settings.SenderEmail : _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent to {To} (subject: '{Subject}').", toEmail, subject);
        }
        catch (SmtpException exception)
        {
            _logger.LogError(exception, "SMTP send failed for recipient {To} (subject: '{Subject}').", toEmail, subject);
            throw;
        }
    }
}
