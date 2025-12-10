using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace RentEZApi.Services;

public class EmailService
{
    private readonly ConfigService _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger, ConfigService config)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmail(string to, string subject, string body)
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
        await smtp.ConnectAsync(_config.GetSmtpHost(), port, SecureSocketOptions.StartTls);

        // Authenticate
        // For Gmail, the "User" is your email address
        await smtp.AuthenticateAsync(_config.EmailFrom(), _config.GetSmtpPassword());

        // 3. Send and disconnect
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
