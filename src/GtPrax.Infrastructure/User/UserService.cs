namespace GtPrax.Infrastructure.User;

using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Services;
using GtPrax.Domain.Models;
using GtPrax.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

internal sealed class UserService : IUserService
{
    private readonly TimeProvider _timeProvider;
    private readonly UserStore _store;
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly SignInManager<UserModel> _signInManager;

    public UserService(
        TimeProvider timeProvider,
        UserStore store,
        IdentityErrorDescriber errorDescriber,
        SignInManager<UserModel> signInManager)
    {
        _timeProvider = timeProvider;
        _store = store;
        _errorDescriber = errorDescriber;
        _signInManager = signInManager;
    }

    public async Task<Result> Create(string email, string name, UserRoleType[] roles)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return Result.Fail(_errorDescriber.DuplicateEmail(email).Description);
        }

        user = new()
        {
            Id = ObjectId.GenerateNewId(),
            UserName = Guid.NewGuid().ToString().Replace("-", string.Empty),
            Email = email,
            Name = name
        };
        foreach (var r in roles)
        {
            user.Claims.Add(new UserClaimModel(ClaimTypes.Role, r.Value));
        }
        var result = await userManager.CreateAsync(user);
        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> CreateAdmin(string email, string password)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return Result.Ok();
        }
        user = new()
        {
            Id = ObjectId.GenerateNewId(),
            UserName = Guid.NewGuid().ToString().Replace("-", string.Empty),
            Email = email,
            Name = "Super User",
            IsEmailConfirmed = true
        };
        user.Claims.Add(new UserClaimModel(ClaimTypes.Role, UserRoleType.Admin.Value));
        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<User?> Find(string id) =>
        (await _signInManager.UserManager.FindByIdAsync(id))?.MapToDomain();

    public async Task<User?> FindByEmail(string email) =>
        (await _signInManager.UserManager.FindByEmailAsync(email))?.MapToDomain();

    public async Task<User[]> GetAll(CancellationToken cancellationToken) =>
        (await _store.GetAllUsers(cancellationToken)).MapToDomain();

    public async Task<Result<SignInAction>> SignIn(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result.Fail(Messages.LoginFailed);
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
        if (signInResult.Succeeded)
        {
            var result = await _store.SetLastLogin(user.Id.ToString(), _timeProvider.GetUtcNow(), cancellationToken);
            return result.Succeeded ? Result.Ok(SignInAction.None) : Result.Fail(result.Errors.Select(e => e.Description));
        }
        else if (signInResult.RequiresTwoFactor)
        {
            return Result.Ok(SignInAction.RequiresTwoFactor);
        }
        else if (signInResult.IsLockedOut)
        {
            return Result.Fail(Messages.AccountTempLockedOut);
        }
        else if (signInResult.IsNotAllowed)
        {
            return Result.Fail(Messages.AccountNotActivated);
        }
        else
        {
            return Result.Fail(Messages.LoginFailed);
        }
    }

    public async Task<Result> SignInTwoFactor(string code, bool rememberClient, CancellationToken cancellationToken)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
        {
            return Result.Fail(Messages.LoginFailed);
        }
        var signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, false, rememberClient);
        if (!signInResult.Succeeded)
        {
            return Result.Fail(Messages.LoginFailed);
        }
        var result = await _store.SetLastLogin(user.Id.ToString(), _timeProvider.GetUtcNow(), cancellationToken);
        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }

    public Task SignOutCurrent() =>
        _signInManager.SignOutAsync();

    public async Task<Result> SetName(string id, string name, CancellationToken cancellationToken)
    {
        var result = await _store.SetName(id, name, cancellationToken);
        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<Result> SetEmail(string id, string newEmail)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var result = await userManager.ChangeEmailAsync(user, newEmail, token);
        if (!result.Succeeded)
        {
            return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> SetPassword(string id, string newPassword)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        IdentityResult result;
        foreach (var validator in userManager.PasswordValidators)
        {
            result = await validator.ValidateAsync(userManager, user, newPassword);
            if (!result.Succeeded)
            {
                return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
        }

        if (!user.IsEmailConfirmed)
        {
            token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        return Result.Ok();
    }

    public async Task<Result> SetRoles(string id, UserRoleType[] roles)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        IdentityResult result;
        var currentStringRoles = await userManager.GetRolesAsync(user);
        if (currentStringRoles.Count < 1)
        {
            result = await userManager.AddToRolesAsync(user, roles.Select(r => r.Value));
            return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
        }

        var currentRoles = UserRoleType.From(currentStringRoles);
        var removeRoles = currentRoles.Except(roles).ToArray();
        var addRoles = roles.Except(currentRoles).ToArray();

        if (removeRoles.Length > 0)
        {
            result = await userManager.RemoveFromRolesAsync(user, removeRoles.Select(r => r.Value));
            if (!result.Succeeded)
            {
                return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        if (addRoles.Length > 0)
        {
            result = await userManager.AddToRolesAsync(user, addRoles.Select(r => r.Value));
            if (!result.Succeeded)
            {
                return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        return Result.Ok();
    }

    public async Task<string?> GenerateResetPasswordToken(string id)
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

    public async Task<Result> VerifyResetPasswordToken(string id, string token)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<UserModel>.ResetPasswordTokenPurpose,
            token);

        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidToken().Description);
        }

        return Result.Ok();
    }

    public async Task<Result> ResetPassword(string id, string token, string newPassword)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<UserModel>.ResetPasswordTokenPurpose,
            token);

        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidToken().Description);
        }

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<string?> GenerateConfirmEmailToken(string id)
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

    public async Task<Result> VerifyConfirmEmailToken(string id, string token)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<UserModel>.ConfirmEmailTokenPurpose,
            token);

        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidToken().Description);
        }

        return Result.Ok();
    }

    public async Task<Result> ConfirmEmail(string id, string token, string newPassword)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        IdentityResult result;
        if (!user.IsEmailConfirmed)
        {
            token = Uri.UnescapeDataString(token);

            var isValid = await userManager.VerifyUserTokenAsync(user,
                userManager.Options.Tokens.EmailConfirmationTokenProvider,
                UserManager<UserModel>.ConfirmEmailTokenPurpose,
                token);

            if (!isValid)
            {
                return Result.Fail(_errorDescriber.InvalidToken().Description);
            }

            result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        foreach (var validator in userManager.PasswordValidators)
        {
            result = await validator.ValidateAsync(userManager, user, newPassword);
            if (!result.Succeeded)
            {
                return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        token = await userManager.GeneratePasswordResetTokenAsync(user);
        result = await userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<string?> GenerateChangeEmailToken(string id, string newEmail)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }
        var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        return Uri.EscapeDataString(token);
    }

    public async Task<Result> ChangeEmail(string id, string token, string newEmail)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.ChangeEmailTokenProvider,
            UserManager<UserModel>.GetChangeEmailTokenPurpose(newEmail),
            token);

        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidToken().Description);
        }

        var result = await userManager.ChangeEmailAsync(user, newEmail, token);
        if (!result.Succeeded)
        {
            return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> ChangePassword(string id, string currentPassword, string newPassword)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        IdentityResult result;
        foreach (var validator in userManager.PasswordValidators)
        {
            result = await validator.ValidateAsync(userManager, user, newPassword);
            if (!result.Succeeded)
            {
                return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
            }
        }

        result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> CheckPassword(string id, string currentPassword)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        if (!await userManager.CheckPasswordAsync(user, currentPassword))
        {
            return Result.Fail(_errorDescriber.PasswordMismatch().Description);
        }

        return Result.Ok();
    }

    public async Task<Result> Deactivate(string id)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        user.Email = $"{user.UserName}@deactivated";
        user.PasswordHash = null;
        user.Name = new string(user.Name.Split(' ').Select(u => u[0]).ToArray()) + "*";
        user.IsEmailConfirmed = false;
        user.DeactivationDate = _timeProvider.GetUtcNow();
        user.LastLoginDate = null;
        user.Claims.Clear();

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Description));
    }

    public async Task<Result<UserTwoFactor>> CreateTwoFactor(string id, string appName)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        var isTwoFactorEnabled = await _signInManager.IsTwoFactorEnabledAsync(user);

        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            var result = await userManager.ResetAuthenticatorKeyAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
            key = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                return Result.Fail(_errorDescriber.DefaultError().Description);
            }
        }

        var uri = GenerateOtpAuthUri(appName, user.Email, key);
        return Result.Ok(new UserTwoFactor(isTwoFactorEnabled, key, uri));
    }

    public async Task<Result> EnableTwoFactor(string id, bool enable, string code)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        var isValid = await userManager.VerifyTwoFactorTokenAsync(user,
            userManager.Options.Tokens.AuthenticatorTokenProvider, code);

        if (!isValid)
        {
            return Result.Fail(_errorDescriber.InvalidToken().Description);
        }

        var result = await userManager.SetTwoFactorEnabledAsync(user, enable);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> ResetTwoFactor(string id)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Result.Fail(Messages.AccountNotFound);
        }

        user.AuthenticatorKey = null;
        var result = await userManager.SetTwoFactorEnabledAsync(user, false);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    private static string GenerateOtpAuthUri(string issuer, string user, string secret)
    {
        var dictionary = new Dictionary<string, string>
        {
            { "secret", secret },
            { "issuer", Uri.EscapeDataString(issuer) },
            { "algorithm","SHA1" },
            { "digits", "6" },
            { "period", "30" }
        };

        var uri = new StringBuilder("otpauth://totp/");
        uri.Append(Uri.EscapeDataString(issuer));
        uri.Append(':');
        uri.Append(Uri.EscapeDataString(user));
        uri.Append('?');
        foreach (var item in dictionary)
        {
            uri.Append(item.Key);
            uri.Append('=');
            uri.Append(item.Value);
            uri.Append('&');
        }

        // remove '&' at the end
        uri.Remove(uri.Length - 1, 1);
        return uri.ToString();
    }
}
