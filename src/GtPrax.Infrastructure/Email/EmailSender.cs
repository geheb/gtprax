namespace GtPrax.Infrastructure.Email;

using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;

internal sealed class EmailSender
{
    private readonly SmtpSettings _emailSettings;

    public EmailSender(IOptions<SmtpSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task Send(string email, string subject, string htmlMessage, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_emailSettings.SenderEmail))
        {
            throw new InvalidOperationException("missing sender email");
        }

        var from = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
        var to = new MailAddress(email);

        using var message = new MailMessage(from, to);
        message.Subject = subject;
        message.Body = htmlMessage;
        message.IsBodyHtml = true;
        message.BodyEncoding = Encoding.UTF8;

        using var client = new SmtpClient(_emailSettings.Server, _emailSettings.Port);
        if (!string.IsNullOrEmpty(_emailSettings.LoginName))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = true; // only Explicit SSL supported
            client.Credentials = new NetworkCredential(_emailSettings.LoginName, _emailSettings.LoginPassword);
        }

        await client.SendMailAsync(message, cancellationToken);
    }
}
