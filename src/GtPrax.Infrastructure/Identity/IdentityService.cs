namespace GtPrax.Infrastructure.Identity;

using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Identity;
using GtPrax.Domain.Entities;
using GtPrax.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

internal sealed class IdentityService : IIdentityService
{
    private static readonly IdentityError NotFound = new() { Code = nameof(Messages.UserNotFound), Description = Messages.UserNotFound };

    private readonly TimeProvider _timeProvider;
    private readonly UserStore _store;
    private readonly IdentityErrorDescriber _errorDescriber;
    private readonly SignInManager<UserModel> _signInManager;

    public IdentityService(
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

    public async Task<IdentityResult> Create(string email, string name, UserRoleType[] roles)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return IdentityResult.Failed(_errorDescriber.DuplicateEmail(email));
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
        return await userManager.CreateAsync(user);
    }

    public async Task<IdentityResult> CreateSuperUser(string email, string password)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return IdentityResult.Success;
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
        return await userManager.CreateAsync(user, password);
    }

    public async Task<User?> FindUser(string id) =>
        (await _signInManager.UserManager.FindByIdAsync(id))?.MapToDomain();

    public async Task<User?> FindUserByEmail(string email) =>
        (await _signInManager.UserManager.FindByEmailAsync(email))?.MapToDomain();

    public async Task<User[]> GetAllUsers(CancellationToken cancellationToken) =>
        (await _store.GetAllUsers(cancellationToken)).MapToDomain();

    public async Task<IdentityResult> SignIn(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = nameof(Messages.LoginFailed), Description = Messages.LoginFailed });
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            return await _store.SetLastLogin(user.Id.ToString(), _timeProvider.GetUtcNow(), cancellationToken);
        }
        else if (result.IsLockedOut)
        {
            return IdentityResult.Failed(new IdentityError { Code = nameof(Messages.AccountTempLockedOut), Description = Messages.AccountTempLockedOut });
        }
        else if (result.IsNotAllowed)
        {
            return IdentityResult.Failed(new IdentityError { Code = nameof(Messages.AccountNotActivated), Description = Messages.AccountNotActivated });
        }
        else
        {
            return IdentityResult.Failed(new IdentityError { Code = nameof(Messages.LoginFailed), Description = Messages.LoginFailed });
        }
    }

    public Task SignOutCurrentUser() =>
        _signInManager.SignOutAsync();

    public Task<IdentityResult> SetName(string id, string name, CancellationToken cancellationToken) =>
        _store.SetName(id, name, cancellationToken);

    public async Task<IdentityResult> SetEmail(string id, string newEmail)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        var token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        var result = await userManager.ChangeEmailAsync(user, newEmail, token);
        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> SetPassword(string id, string newPassword)
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
            result = await validator.ValidateAsync(userManager, user, newPassword);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            return result;
        }

        if (!user.IsEmailConfirmed)
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

    public async Task<IdentityResult> SetRoles(string id, UserRoleType[] roles)
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
            return await userManager.AddToRolesAsync(user, roles.Select(r => r.Value));
        }

        var currentRoles = UserRoleType.From(currentStringRoles);
        var removeRoles = currentRoles.Except(roles).ToArray();
        var addRoles = roles.Except(currentRoles).ToArray();

        if (removeRoles.Length > 0)
        {
            var result = await userManager.RemoveFromRolesAsync(user, removeRoles.Select(r => r.Value));
            if (!result.Succeeded)
            {
                return result;
            }
        }

        if (addRoles.Length > 0)
        {
            var result = await userManager.AddToRolesAsync(user, addRoles.Select(r => r.Value));
            if (!result.Succeeded)
            {
                return result;
            }
        }

        return IdentityResult.Success;
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

    public async Task<IdentityResult> VerifyResetPasswordToken(string id, string token)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<UserModel>.ResetPasswordTokenPurpose,
            token);

        if (!isValid)
        {
            return IdentityResult.Failed(_errorDescriber.InvalidToken());
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ResetPassword(string id, string token, string newPassword)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<UserModel>.ResetPasswordTokenPurpose,
            token);

        if (!isValid)
        {
            return IdentityResult.Failed(_errorDescriber.InvalidToken());
        }

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
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

    public async Task<IdentityResult> VerifyConfirmEmailToken(string id, string token)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<UserModel>.ConfirmEmailTokenPurpose,
            token);

        if (!isValid)
        {
            return IdentityResult.Failed(_errorDescriber.InvalidToken());
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ConfirmEmail(string id, string token, string newPassword)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        if (!user.IsEmailConfirmed)
        {
            token = Uri.UnescapeDataString(token);

            var isValid = await userManager.VerifyUserTokenAsync(user,
                userManager.Options.Tokens.EmailConfirmationTokenProvider,
                UserManager<UserModel>.ConfirmEmailTokenPurpose,
                token);

            if (!isValid)
            {
                return IdentityResult.Failed(_errorDescriber.InvalidToken());
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        foreach (var validator in userManager.PasswordValidators)
        {
            var result = await validator.ValidateAsync(userManager, user, newPassword);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        token = await userManager.GeneratePasswordResetTokenAsync(user);
        return await userManager.ResetPasswordAsync(user, token, newPassword);
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

    public async Task<IdentityResult> ChangeEmail(string id, string token, string newEmail)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        token = Uri.UnescapeDataString(token);

        var isValid = await userManager.VerifyUserTokenAsync(user,
            userManager.Options.Tokens.ChangeEmailTokenProvider,
            UserManager<UserModel>.GetChangeEmailTokenPurpose(newEmail),
            token);

        if (!isValid)
        {
            return IdentityResult.Failed(_errorDescriber.InvalidToken());
        }

        var result = await userManager.ChangeEmailAsync(user, newEmail, token);
        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ChangePassword(string id, string currentPassword, string newPassword)
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
            result = await validator.ValidateAsync(userManager, user, newPassword);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> CheckPassword(string id, string currentPassword)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        if (!await userManager.CheckPasswordAsync(user, currentPassword))
        {
            return IdentityResult.Failed(_errorDescriber.PasswordMismatch());
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> Deactivate(string id)
    {
        var userManager = _signInManager.UserManager;
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(NotFound);
        }

        user.Email = $"{user.UserName}@deactivated";
        user.PasswordHash = null;
        user.Name = new string(user.Name.Split(' ').Select(u => u[0]).ToArray()) + "*";
        user.IsEmailConfirmed = false;
        user.DeactivationDate = _timeProvider.GetUtcNow();
        user.LastLoginDate = null;
        user.Claims.Clear();

        return await userManager.UpdateAsync(user);
    }
}
