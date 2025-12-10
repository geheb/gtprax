namespace GtPrax.Infrastructure.Repositories;

using System.Text;
using System.Web;
using GtPrax.Application.Converter;
using GtPrax.Application.Models;
using GtPrax.Application.Repositories;
using GtPrax.Application.Services;
using GtPrax.Infrastructure.Database;
using GtPrax.Infrastructure.Database.Entities;
using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class UserRepository : IUserRepository
{
    private readonly ILogger _logger;
    private readonly AppDbContext _dbContext;
    private readonly UserManager<IdentityUserGuid> _userManager;
    private readonly IHttpContextAccessor _httpContext;
    private readonly LinkGenerator _linkGenerator;
    private readonly IEmailValidator _emailValidator;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly SignInManager<IdentityUserGuid> _signInManager;
    private readonly double _defaultLockoutMinutes;

    public UserRepository(
        ILogger<UserRepository> logger,
        AppDbContext dbContext,
        UserManager<IdentityUserGuid> userManager,
        IHttpContextAccessor httpContext,
        LinkGenerator linkGenerator,
        IEmailValidator emailValidator,
        IDataProtectionProvider dataProtectionProvider,
        SignInManager<IdentityUserGuid> signInManager,
        IOptions<IdentityOptions> identityOptions)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
        _httpContext = httpContext;
        _linkGenerator = linkGenerator;
        _emailValidator = emailValidator;
        _dataProtectionProvider = dataProtectionProvider;
        _signInManager = signInManager;
        _defaultLockoutMinutes = (identityOptions.Value ?? new IdentityOptions()).Lockout.DefaultLockoutTimeSpan.TotalMinutes;
    }

    public async Task<string?> SignIn(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("user {Email} not found", email);
            return "Ungültiger Anmeldeversuch. Bitte E-Mail und/oder Passwort überprüfen.";
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            user.LastLogin = DateTimeOffset.UtcNow;
            await _signInManager.UserManager.UpdateAsync(user);

            _logger.LogInformation("user {Id} logged in", user.Id);
            return null;
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning("user {Id} is locked out", user.Id);
            return $"Dein Login ist vorübergehend gesperrt, versuche in {_defaultLockoutMinutes} Minuten wieder.";
        }
        else if (result.IsNotAllowed)
        {
            _logger.LogWarning("user {Id} is not allowed to log in", user.Id);
            return "Login fehlgeschlagen, du kannst dich leider noch nicht anmelden.";
        }
        else
        {
            _logger.LogWarning("user {Id} failed to log in", user.Id);
            return "Login fehlgeschlagen, bitte E-Mail und/oder Passwort überprüfen.";
        }
    }

    public async Task SignOutCurrentUser()
    {
        if (_httpContext.HttpContext == null)
        {
            return;
        }

        var userId = _httpContext.HttpContext.User.GetId();

        await _signInManager.SignOutAsync();

        _logger.LogInformation("user {Id} logged out", userId);
    }

    public async Task<bool> NotifyPasswordForgotten(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("user {Email} not found", email);
            return false;
        }

        return await NotifyConfirmPasswordForgotten(user, cancellationToken);
    }

    public async Task<string[]?> Update(UserDto dto, string? password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(dto.Id.ToString());
        if (user == null)
        {
            return ["Benutzer wurde nicht gefunden"];
        }

        if (dto.Email != null && !dto.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
        {
            var isValid = await _emailValidator.Validate(dto.Email, cancellationToken);
            if (!isValid)
            {
                return ["Die E-Mail ist ungültig"];
            }

            var result = await _userManager.SetEmailAsync(user, dto.Email);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        if (!string.IsNullOrEmpty(password))
        {
            foreach (var validator in _userManager.PasswordValidators)
            {
                var r = await validator.ValidateAsync(_userManager, user, password);
                if (!r.Succeeded)
                {
                    return r.Errors.Select(e => e.Description).ToArray();
                }
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }

            if (!user.EmailConfirmed)
            {
                token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    return result.Errors.Select(e => e.Description).ToArray();
                }
            }
        }

        var count = 0;
        if (!(dto.Name ?? string.Empty).Equals(user.Name, StringComparison.Ordinal))
        {
            user.Name = dto.Name;
            count++;
        }

        if (count > 0)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var removeRoles = roles.Except(dto.Roles ?? []).ToArray();
        var addRoles = dto.Roles != null ? dto.Roles.Except(roles).ToArray() : [];

        if (removeRoles.Length > 0)
        {
            var result = await _userManager.RemoveFromRolesAsync(user, removeRoles);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        if (addRoles.Length > 0)
        {
            var result = await _userManager.AddToRolesAsync(user, addRoles);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        return null;
    }

    public async Task<string[]?> Update(Guid id, string name)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return ["Benutzer wurde nicht gefunden"];
        }

        var hasChanges = false;

        if (!(name ?? string.Empty).Equals(user.Name, StringComparison.Ordinal))
        {
            hasChanges = true;
            user.Name = name;
        }

        if (hasChanges)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).ToArray();
            }
        }

        return null;
    }

    public async Task<string?> VerfiyChangePassword(Guid id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogWarning("user {Id} not found", id);
            return null;
        }

        token = HttpUtility.UrlDecode(token);

        var isTokenValid = await _userManager.VerifyUserTokenAsync(user,
            _userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<IdentityUserGuid>.ResetPasswordTokenPurpose,
            token);

        if (!isTokenValid)
        {
            _logger.LogError("verify token for user {Id} failed", id);
            return default;
        }

        return user.Email;
    }

    public async Task<(string[]? Error, string? Email)> ChangePassword(Guid id, string? token, string password)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogWarning("user {Id} not found", id);
            return (null, null);
        }

        if (string.IsNullOrEmpty(token))
        {
            token = await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        else
        {
            token = HttpUtility.UrlDecode(token);

            var isTokenValid = await _userManager.VerifyUserTokenAsync(user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<IdentityUserGuid>.ResetPasswordTokenPurpose,
                token);

            if (!isTokenValid)
            {
                _logger.LogError("verify token for user {Id} failed", id);
                return (null, null);
            }
        }

        foreach (var validator in _userManager.PasswordValidators)
        {
            var r = await validator.ValidateAsync(_userManager, user, password);
            if (!r.Succeeded)
            {
                return (r.Errors.Select(r => r.Description).ToArray(), user.Email);
            }
        }

        var result = await _userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
        {
            return (result.Errors.Select(r => r.Description).ToArray(), user.Email);
        }

        return (null, user.Email);
    }

    public async Task<(string? Error, bool IsFatal)> NotifyConfirmChangeEmail(Guid id, string newEmail, string currentPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return ("Benutzer wurde nicht gefunden", true);
        }

        if (await _userManager.FindByEmailAsync(newEmail) != null)
        {
            return ("Die neue E-Mail-Adresse kann nicht genutzt werden", false);
        }

        if (!await _emailValidator.Validate(newEmail, cancellationToken))
        {
            return ("Die neue E-Mail-Adresse ist ungültig", false);
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, currentPassword);
        if (result != PasswordVerificationResult.Success)
        {
            return ("Das angegebene Passwort stimmt nicht überein", false);
        }

        if (!await NotifyConfirmChangeEmail(user, newEmail, cancellationToken))
        {
            return ("Fehler beim Speichern", true);
        }

        return (null, false);
    }

    public async Task<string?> VerifyConfirmRegistration(Guid id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogError("user {Id} not found", id);
            return null;
        }

        token = HttpUtility.UrlDecode(token);

        var isUserTokenValid = await _userManager.VerifyUserTokenAsync(user,
            _userManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<IdentityUserGuid>.ConfirmEmailTokenPurpose,
            token);

        if (!isUserTokenValid)
        {
            _logger.LogError("user {Id} has invalid token", id);
            return null;
        }

        return user.Email;
    }

    public async Task<string[]?> ConfirmRegistrationAndSetPassword(Guid id, string token, string password)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogError("user {Id} not found", id);
            return ["Benutzer wurde nicht gefunden"];
        }

        token = HttpUtility.UrlDecode(token);

        var isUserTokenValid = await _userManager.VerifyUserTokenAsync(user,
            _userManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<IdentityUserGuid>.ConfirmEmailTokenPurpose,
            token);

        if (!isUserTokenValid)
        {
            _logger.LogError("user {Id} has invalid token", id);
            return ["Der Link ist ungültig oder abgelaufen."];
        }

        var identityResult = await _userManager.ConfirmEmailAsync(user, token);
        if (!identityResult.Succeeded)
        {
            return identityResult.Errors.Select(r => r.Description).ToArray();
        }

        var result = await ChangePassword(id, null, password);
        return result.Error;
    }

    public async Task<bool> NotifyConfirmRegistration(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            _logger.LogError("user {Id} not found", id);
            return false;
        }

        if (user.EmailConfirmed)
        {
            _logger.LogInformation("user {Id} is already confirmed", id);
            return true;
        }

        return await NotifyConfirmRegistration(user, cancellationToken);
    }

    public async Task<UserDto[]> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .Include(e => e.UserRoles!)
            .ThenInclude(e => e.Role)
            .Where(e => e.RemovedOn == null)
            .OrderBy(e => e.Name)
            .ToArrayAsync(cancellationToken);

        var ec = new EmailConverter();
        var dc = new GermanDateTimeConverter();

        return users.Select(e => e.ToDto(ec, dc)).ToArray();
    }

    public async Task<UserDto?> Find(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Include(e => e.UserRoles!)
            .ThenInclude(e => e.Role)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (user == null)
        {
            return null;
        }

        var ec = new EmailConverter();
        var dc = new GermanDateTimeConverter();

        return user.ToDto(ec, dc);
    }

    public async Task<string[]?> Create(UserDto dto, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email!);
        if (user != null)
        {
            return ["Der Benutzer mit der E-Mail-Adresse existiert bereits."];
        }

        if (dto.Email == null || !await _emailValidator.Validate(dto.Email, cancellationToken))
        {
            return ["Die E-Mail-Adresse ist ungültig."];
        }

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var id = _dbContext.GeneratePk();

        user = new IdentityUserGuid
        {
            Id = id,
            UserName = id.ToString("N"),
            Name = dto.Name,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return result.Errors.Select(e => e.Description).ToArray();
        }

        result = await _userManager.AddToRolesAsync(user, dto.Roles ?? []);
        if (!result.Succeeded)
        {
            return result.Errors.Select(e => e.Description).ToArray();
        }

        if (!await NotifyConfirmRegistration(user, cancellationToken))
        {
            return ["Fehler beim Speichern"];
        }

        await transaction.CommitAsync(cancellationToken);

        return null;
    }

    public async Task<string?> ConfirmChangeEmail(Guid id, string token, string encodedEmail)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return null;
        }

        string? newEmail = null;

        try
        {
            var protector = _dataProtectionProvider.CreateProtector(user.SecurityStamp!);

            newEmail = Encoding.UTF8.GetString(protector.Unprotect(Convert.FromBase64String(encodedEmail)));

            token = HttpUtility.UrlDecode(token);

            var isUserTokenValid = await _userManager.VerifyUserTokenAsync(user,
                _userManager.Options.Tokens.ChangeEmailTokenProvider,
                UserManager<IdentityUserGuid>.GetChangeEmailTokenPurpose(newEmail),
                token);

            if (!isUserTokenValid)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "change email failed");
            return null;
        }

        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
        if (!result.Succeeded)
        {
            var error = string.Join(";", result.Errors.Select(e => e.Description));
            _logger.LogError("change email failed: {Error}", error);

            return null;
        }

        return newEmail;
    }

    public async Task<bool> Remove(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return false;
        }

        var notifications = await _dbContext.AccountNotifications.Where(e => e.UserId == id).ToArrayAsync(cancellationToken);
        if (notifications.Length > 0)
        {
            _dbContext.AccountNotifications.RemoveRange(notifications);
            if (await _dbContext.SaveChangesAsync(cancellationToken) < 1)
            {
                return false;
            }
        }

        var result = await _userManager.RemovePasswordAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("remove password for user {Id} failed", id);
            return false;
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Any())
        {
            result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                _logger.LogError("remove roles for user {Id} failed", id);
                return false;
            }
        }

        user.EmailConfirmed = false;
        user.Email = user.UserName + "@removed";
        user.RemovedOn = DateTimeOffset.UtcNow;
        user.Name = new string(user.Name?.Split(' ', '-').Select(u => u[0]).ToArray()) + "*";

        result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("anonymize user {Id} failed", id);
            return false;
        }

        return true;
    }

    private async Task<bool> NotifyConfirmRegistration(IdentityUserGuid user, CancellationToken cancellationToken)
    {
        if (_dbContext.AccountNotifications == null ||
            _httpContext.HttpContext == null)
        {
            return false;
        }

        var type = AccountEmailTemplate.ConfirmRegistration;

        var entity = await _dbContext.AccountNotifications
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedOn)
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.Type == (int)type, cancellationToken);

        if (entity != null && !entity.SentOn.HasValue)
        {
            _logger.LogInformation("notification for user {Id} is pending", user.Id);
            return true;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        token = HttpUtility.UrlEncode(token);

        var callbackUrl = _linkGenerator.GetUriByPage(_httpContext.HttpContext, "/Login/ConfirmRegistration", null, new { id = user.Id, token });

        entity = new AccountNotification
        {
            Id = _dbContext.GeneratePk(),
            UserId = user.Id,
            Type = (int)type,
            CreatedOn = DateTimeOffset.UtcNow,
            CallbackUrl = callbackUrl
        };

        _dbContext.AccountNotifications.Add(entity);

        if (await _dbContext.SaveChangesAsync(cancellationToken) < 1)
        {
            _logger.LogInformation("save for user {Id} failed", user.Id);
            return false;
        }
        return true;
    }

    private async Task<bool> NotifyConfirmPasswordForgotten(IdentityUserGuid user, CancellationToken cancellationToken)
    {
        if (_dbContext.AccountNotifications == null ||
            _httpContext.HttpContext == null)
        {
            return false;
        }

        var type = AccountEmailTemplate.ConfirmPasswordForgotten;

        var entity = await _dbContext.AccountNotifications
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedOn)
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.Type == (int)type, cancellationToken);

        if (entity != null && !entity.SentOn.HasValue)
        {
            _logger.LogInformation("notification for user {Id} is pending", user.Id);
            return true;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = HttpUtility.UrlEncode(token);

        var callbackUrl = _linkGenerator.GetUriByPage(_httpContext.HttpContext, "/Login/ConfirmChangePassword", null, new { id = user.Id, token });

        entity = new AccountNotification
        {
            Id = _dbContext.GeneratePk(),
            UserId = user.Id,
            Type = (int)type,
            CreatedOn = DateTimeOffset.UtcNow,
            CallbackUrl = callbackUrl
        };

        _dbContext.AccountNotifications.Add(entity);

        if (await _dbContext.SaveChangesAsync(cancellationToken) < 1)
        {
            _logger.LogInformation("save for user {Id} failed", user.Id);
            return false;
        }
        return true;
    }

    private async Task<bool> NotifyConfirmChangeEmail(IdentityUserGuid user, string newEmail, CancellationToken cancellationToken)
    {
        if (_dbContext.AccountNotifications == null ||
            _httpContext.HttpContext == null)
        {
            return false;
        }

        var type = AccountEmailTemplate.ConfirmChangeEmail;

        var pendingNotifications = await _dbContext.AccountNotifications
            .Where(e => e.UserId == user.Id && e.Type == (int)type && e.SentOn == null)
            .ToArrayAsync(cancellationToken);

        // disable pending notifications
        if (pendingNotifications.Length > 0)
        {
            foreach (var e in pendingNotifications)
            {
                e.SentOn = DateTimeOffset.MinValue;
            }
            if (await _dbContext.SaveChangesAsync(cancellationToken) < 1)
            {
                _logger.LogInformation("save pending notifications for {Id} failed", user.Id);
                return false;
            }
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        token = HttpUtility.UrlEncode(token);

        var protector = _dataProtectionProvider.CreateProtector(user.SecurityStamp!);
        var newEmailProtected = Convert.ToBase64String(protector.Protect(Encoding.UTF8.GetBytes(newEmail)));

        var callbackUrl = _linkGenerator.GetUriByPage(_httpContext.HttpContext, "/Login/ConfirmChangeEmail", null,
            new { id = user.Id, token, email = newEmailProtected });

        var newNotify = new AccountNotification
        {
            Id = _dbContext.GeneratePk(),
            UserId = user.Id,
            Type = (int)type,
            CreatedOn = DateTimeOffset.UtcNow,
            CallbackUrl = callbackUrl
        };

        _dbContext.AccountNotifications.Add(newNotify);

        if (await _dbContext.SaveChangesAsync(cancellationToken) < 1)
        {
            _logger.LogInformation("save notification for user {Id} failed", user.Id);
            return false;
        }

        return true;
    }
}
