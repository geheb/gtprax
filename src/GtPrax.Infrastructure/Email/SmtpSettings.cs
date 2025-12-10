namespace GtPrax.Infrastructure.Email;

public sealed class SmtpSettings
{
    public string? Server { get; set; }
    public int Port { get; set; }
    public string? LoginName { get; set; }
    public string? LoginPassword { get; set; }
    public string? SenderName { get; set; }
    public string? SenderEmail { get; set; }
}
