using System.Net.Mail;
using System.Threading.Tasks;  // For Task return types
using Microsoft.Extensions.Configuration;  // For IConfiguration

public class EmailService : IEmailService
{
    private readonly EmailSettings? _emailSettings;

    public EmailService(IConfiguration config)
    {
        _emailSettings = config.GetSection("EmailSettings").Get<EmailSettings>();
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (_emailSettings == null)
        {
            throw new InvalidOperationException("Email settings are not configured.");
        }

        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        {
            Credentials = new System.Net.NetworkCredential(_emailSettings.SenderEmail,
                _emailSettings.SenderPassword),
            EnableSsl = true
        };

        if (string.IsNullOrEmpty(_emailSettings.SenderEmail))
        {
            throw new InvalidOperationException("Sender email is not configured.");
        }

        var mailMessage = new MailMessage(_emailSettings.SenderEmail, to, subject, body);
        await client.SendMailAsync(mailMessage);
    }
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}