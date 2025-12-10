namespace GtPrax.Infrastructure.Email;

using System.Net.Mail;
using System.Text;
using System.Web;
using GtPrax.Application.Models;
using GtPrax.Infrastructure.Database;
using GtPrax.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class AccountNotificationWorker
{
    private readonly AppDbContext _dbContext;
    private readonly AppSettings _appSettings;
    private readonly ILogger _logger;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly AccountEmailTemplateRenderer _accountEmailTemplateRenderer = new();
    private readonly EmailSender _emailSender;

    private readonly TimeSpan _confirmEmailTimeout, _changeEmailPassTimeout;

    public AccountNotificationWorker(
        AppDbContext dbContext,
        IOptions<AppSettings> appSettingsOptions,
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> confirmEmailOptions,
        IOptions<DataProtectionTokenProviderOptions> changeEmailPassOptions,
        ILogger<AccountNotificationWorker> logger,
        IDataProtectionProvider dataProtectionProvider,
        EmailSender emailSender)
    {
        _dbContext = dbContext;
        _appSettings = appSettingsOptions.Value;
        _logger = logger;
        _dataProtectionProvider = dataProtectionProvider;
        _emailSender = emailSender;

        _confirmEmailTimeout = confirmEmailOptions.Value.TokenLifespan;
        _changeEmailPassTimeout = changeEmailPassOptions.Value.TokenLifespan;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var entities = await _dbContext.AccountNotifications
            .Include(e => e.User)
            .Where(e => e.SentOn == null)
            .Take(32)
            .ToArrayAsync(cancellationToken);

        if (entities.Length == 0)
        {
            return;
        }

        var count = 0;

        foreach (var entity in entities)
        {
            if (await HandleUser(entity, cancellationToken))
            {
                count++;
            }
        }

        if (count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static string Format(TimeSpan span) =>
        span.TotalDays > 1 ? $"{span.TotalDays} Tage" : $"{span.TotalHours} Stunden";

    private string GetTimeout(AccountEmailTemplate template) =>
        template switch
        {
            AccountEmailTemplate.ConfirmRegistration => Format(_confirmEmailTimeout),
            AccountEmailTemplate.ConfirmPasswordForgotten => Format(_changeEmailPassTimeout),
            AccountEmailTemplate.ConfirmChangeEmail => Format(_changeEmailPassTimeout),
            _ => throw new NotImplementedException($"unknown {nameof(AccountEmailTemplate)} {template}")
        };

    private string GetTitle(AccountEmailTemplate template) =>
        template switch
        {
            AccountEmailTemplate.ConfirmRegistration => "GT Prax - Registrierung",
            AccountEmailTemplate.ConfirmPasswordForgotten => "GT Prax - Passwort",
            AccountEmailTemplate.ConfirmChangeEmail => "GT Prax - E-Mail-Adresse",
            _ => throw new NotImplementedException($"unknown {nameof(AccountEmailTemplate)} {template}")
        };

    private async Task<bool> HandleUser(AccountNotification entity, CancellationToken cancellationToken)
    {
        var template = (AccountEmailTemplate)entity.Type;
        if (entity.User == null)
        {
            throw new InvalidOperationException("user cant't be null");
        }

        var model = new
        {
            title = GetTitle(template),
            name = entity.User.Name!.Split(' ')[0],
            link = entity.CallbackUrl,
            timeout = GetTimeout(template),
            signature = _appSettings.Signature
        };

        var targetEmail = entity.User.Email!;

        if (template == AccountEmailTemplate.ConfirmChangeEmail)
        {
            var email = GetEmailFromUrl(entity.CallbackUrl!, entity.User.SecurityStamp!);
            if (!string.IsNullOrEmpty(email))
            {
                targetEmail = email;
            }
        }

        var message = await _accountEmailTemplateRenderer.Render(template, model);

        try
        {
            await _emailSender.Send(targetEmail, model.title, message, cancellationToken);
            entity.SentOn = DateTimeOffset.UtcNow;
            return true;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "send email {Template} for user {UserId} failed", template, entity.User.Id);
        }

        return false;
    }

    private string? GetEmailFromUrl(string callbackUrl, string securityStamp)
    {
        var query = HttpUtility.ParseQueryString(callbackUrl);

        var encodedEmail = query.Get("email");
        if (string.IsNullOrEmpty(encodedEmail))
        {
            return null;
        }

        var protector = _dataProtectionProvider.CreateProtector(securityStamp);

        var decodedEmail = Encoding.UTF8.GetString(protector.Unprotect(Convert.FromBase64String(encodedEmail)));

        return decodedEmail;
    }
}
