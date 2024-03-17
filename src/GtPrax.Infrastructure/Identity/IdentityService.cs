namespace GtPrax.Infrastructure.Identity;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Identity;
using GtPrax.Domain.Entities;
using Microsoft.AspNetCore.Identity;

internal sealed class IdentityService : IIdentityService
{
    private static readonly IdentityError NotFound = new() { Code = "NotFound", Description = "Der Benutzer wurde nicht gefunden." };

    private readonly TimeProvider _timeProvider;
    private readonly ApplicationUserStore _store;
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        TimeProvider timeProvider,
        ApplicationUserStore store,
        IdentityErrorDescriber errorDescriber,
        SignInManager<ApplicationUser> signInManager)
    {
        _timeProvider = timeProvider;
        _store = store;
        _errorDescriber = errorDescriber;
        _signInManager = signInManager;
    }

    public async Task<User?> FindUser(string id) =>
        (await _signInManager.UserManager.FindByIdAsync(id))?.MapToUser();

    public async Task<User[]> GetAllUsers(CancellationToken cancellationToken) =>
        (await _store.GetAllUsers(cancellationToken)).MapToUsers();

    public async Task<IdentityResult> SignIn(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "LoginFailed", Description = "Die Anmeldung ist fehlgeschlagen." });
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            return await _store.SetLastLogin(user.Id.ToString(), _timeProvider.GetUtcNow(), cancellationToken);
        }
        else if (result.IsLockedOut)
        {
            return IdentityResult.Failed(new IdentityError { Code = "LockedOut", Description = "Dein Login ist vorübergehend gesperrt." });
        }
        else if (result.IsNotAllowed)
        {
            return IdentityResult.Failed(new IdentityError { Code = "NotActivated", Description = "Dein Login ist nicht freigeschaltet." });
        }
        else
        {
            return IdentityResult.Failed(new IdentityError { Code = "LoginFailed", Description = "Die Anmeldung ist fehlgeschlagen." });
        }
    }

    public Task SignOutCurrentUser() =>
        _signInManager.SignOutAsync();

    public Task<IdentityResult> SetName(string id, string name, CancellationToken cancellationToken) =>
        _store.SetName(id, name, cancellationToken);

    public async Task<IdentityResult> ResetPassword(string id, string password)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        IdentityResult result;
        foreach (var validator in userManager.PasswordValidators)
        {
            result = await validator.ValidateAsync(userManager, user, password);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        result = await userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
        {
            return result;
        }

        if (!user.EmailConfirmed)
        {
            token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> SetRoles(string id, UserRole[] roles)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        var currentStringRoles = await userManager.GetRolesAsync(user);
        if (currentStringRoles.Count < 1)
        {
            return await userManager.AddToRolesAsync(user, roles.Select(r => r.ToString()));
        }

        var currentRoles = currentStringRoles.Select(Enum.Parse<UserRole>).ToArray();
        var removeRoles = currentRoles.Except(roles).ToArray();
        var addRoles = roles.Except(currentRoles).ToArray();

        if (removeRoles.Length > 0)
        {
            var result = await userManager.RemoveFromRolesAsync(user, removeRoles.Select(r => r.ToString()));
            if (!result.Succeeded)
            {
                return result;
            }
        }

        if (addRoles.Length > 0)
        {
            var result = await userManager.AddToRolesAsync(user, addRoles.Select(r => r.ToString()));
            if (!result.Succeeded)
            {
                return result;
            }
        }

        return IdentityResult.Success;
    }

    public async Task<string?> GeneratePasswordResetToken(string id)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return Uri.EscapeDataString(token);
    }

    public async Task<string?> GenerateEmailConfirmationToken(string id)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return Uri.EscapeDataString(token);
    }

    public async Task<IdentityResult> VerifyPasswordResetToken(string id, string token)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isTokenValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<ApplicationUser>.ResetPasswordTokenPurpose,
            token);

        if (!isTokenValid)
        {
            return IdentityResult.Failed(_errorDescriber.InvalidToken());
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> VerifyEmailConfirmationToken(string id, string token)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isTokenValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<ApplicationUser>.ConfirmEmailTokenPurpose,
            token);

        if (!isTokenValid)
        {
            return IdentityResult.Failed(_errorDescriber.InvalidToken());
        }

        return IdentityResult.Success;
    }
}
