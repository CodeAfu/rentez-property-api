using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace RentEZApi.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly ConfigService _config;

    public EmailService(ILogger<EmailService> logger, ConfigService config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.EmailFrom())); // Call method
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();

            // Connect
            // Parse the port string to int
            int port = int.Parse(_config.GetSmtpPort());
            await smtp.ConnectAsync(_config.GetSMTPHost(), port, SecureSocketOptions.StartTls);

            // Authenticate
            // For Gmail, the "User" is your email address
            await smtp.AuthenticateAsync(_config.EmailFrom(), _config.GetSMTPPassword());

            // 3. Send and disconnect
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Recipient}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", to);
            throw;
        }
    }
}
