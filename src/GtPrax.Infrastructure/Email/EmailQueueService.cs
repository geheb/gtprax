namespace GtPrax.Infrastructure.Email;

using System.Reflection;
using System.Threading.Tasks;
using GtPrax.Application.Email;
using GtPrax.Application.Options;
using GtPrax.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

internal sealed class EmailQueueService : IEmailQueueService
{
    private static readonly Assembly CurrentAssembly = typeof(EmailQueueService).Assembly;
    private readonly TimeProvider _timeProvider;
    private readonly EmailQueueStore _store;
    private readonly TemplateRenderer _templateRenderer;
    private readonly TimeSpan _confirmEmailTimeout;
    private readonly TimeSpan _changeEmailOrPasswordTimeout;
    private readonly string _signature;

    public EmailQueueService(
        TimeProvider timeProvider,
        IOptions<AppOptions> appOptions,
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> confirmEmailOptions,
        IOptions<DataProtectionTokenProviderOptions> changeEmailOrPasswordOptions,
        EmailQueueStore store)
    {
        _signature = appOptions.Value.Signature;
        _timeProvider = timeProvider;
        _store = store;
        _templateRenderer = new(CurrentAssembly);
        _confirmEmailTimeout = confirmEmailOptions.Value.TokenLifespan;
        _changeEmailOrPasswordTimeout = changeEmailOrPasswordOptions.Value.TokenLifespan;
    }

    public async Task EnqueueConfirmEmail(string email, string name, string link, CancellationToken cancellationToken)
    {
        var model = new
        {
            title = "GT Prax - E-Mail-Adresse bestätigen",
            name,
            link,
            timeout = Format(_confirmEmailTimeout),
            signature = _signature
        };

        var message = await _templateRenderer.Render("ConfirmEmail.html", model);
        var entity = new EmailQueueModel
        {
            Id = ObjectId.GenerateNewId(),
            CreatedOn = _timeProvider.GetUtcNow(),
            Email = email,
            Message = message,
            Subject = model.title
        };

        await _store.Add(entity, cancellationToken);
    }

    public async Task EnqueueResetPassword(string email, string name, string link, CancellationToken cancellationToken)
    {
        var model = new
        {
            title = "GT Prax - Passwort vergessen",
            name,
            link,
            timeout = Format(_changeEmailOrPasswordTimeout),
            signature = _signature
        };

        var message = await _templateRenderer.Render("ResetPassword.html", model);
        var entity = new EmailQueueModel
        {
            Id = ObjectId.GenerateNewId(),
            CreatedOn = _timeProvider.GetUtcNow(),
            Email = email,
            Message = message,
            Subject = model.title
        };

        await _store.Add(entity, cancellationToken);
    }

    public async Task EnqueueChangeEmail(string email, string name, string link, CancellationToken cancellationToken)
    {
        var model = new
        {
            title = "GT Prax - E-Mail-Adresse ändern",
            name,
            link,
            timeout = Format(_changeEmailOrPasswordTimeout),
            signature = _signature
        };

        var message = await _templateRenderer.Render("ChangeEmail.html", model);
        var entity = new EmailQueueModel
        {
            Id = ObjectId.GenerateNewId(),
            CreatedOn = _timeProvider.GetUtcNow(),
            Email = email,
            Message = message,
            Subject = model.title
        };

        await _store.Add(entity, cancellationToken);
    }

    private static string Format(TimeSpan span) => span.TotalDays > 1 ? $"{span.TotalDays} Tage" : $"{span.TotalHours} Stunden";
}
