namespace GtPrax.Infrastructure.Email;

using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

internal sealed class EmailDispatchService
{
    private readonly ILogger _logger;
    private readonly IEmailSender _emailSender;
    private readonly EmailQueueStore _store;

    public EmailDispatchService(
        ILogger<EmailDispatchService> logger,
        IEmailSender emailSender,
        EmailQueueStore store)
    {
        _logger = logger;
        _emailSender = emailSender;
        _store = store;
    }

    internal async Task HandleEmails(CancellationToken cancellationToken)
    {
        var entities = await _store.GetNotSentLimited(cancellationToken);
        if (entities.Count < 1)
        {
            return;
        }

        foreach (var entity in entities)
        {
            try
            {
                await _emailSender.SendEmailAsync(entity.Email, entity.Subject, entity.Message);
                await _store.UpdateSentOn(entity.Id, cancellationToken);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Send email {Id} failed", entity.Id);
            }
        }
    }
}
