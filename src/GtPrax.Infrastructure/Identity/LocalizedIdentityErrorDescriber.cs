namespace GtPrax.Infrastructure.Identity;

using System.Globalization;
using Microsoft.AspNetCore.Identity;

public sealed class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
#pragma warning disable CA1863
    public override IdentityError DuplicateEmail(string email) =>
        new()
        {
            Code = nameof(DuplicateEmail),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.DuplicateEmail, email)
        };

    public override IdentityError DuplicateUserName(string userName) =>
        new()
        {
            Code = nameof(DuplicateUserName),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.DuplicateUserName, userName)
        };

    public override IdentityError InvalidEmail(string? email) =>
        new()
        {
            Code = nameof(InvalidEmail),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.InvalidEmail, email)
        };

    public override IdentityError DuplicateRoleName(string role) =>
        new()
        {
            Code = nameof(DuplicateRoleName),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.DuplicateRoleName, role)
        };

    public override IdentityError InvalidRoleName(string? role) =>
        new()
        {
            Code = nameof(InvalidRoleName),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.InvalidRoleName, role)
        };

    public override IdentityError InvalidToken() =>
        new()
        {
            Code = nameof(InvalidToken),
            Description = Messages.InvalidToken
        };

    public override IdentityError InvalidUserName(string? userName) =>
        new()
        {
            Code = nameof(InvalidUserName),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.InvalidUserName, userName)
        };

    public override IdentityError LoginAlreadyAssociated() =>
        new()
        {
            Code = nameof(LoginAlreadyAssociated),
            Description = Messages.LoginAlreadyAssociated
        };

    public override IdentityError PasswordMismatch() =>
        new()
        {
            Code = nameof(PasswordMismatch),
            Description = Messages.PasswordMismatch
        };

    public override IdentityError PasswordRequiresDigit() =>
        new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = Messages.PasswordRequiresDigit
        };

    public override IdentityError PasswordRequiresLower() =>
        new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = Messages.PasswordRequiresLower
        };

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = Messages.PasswordRequiresNonAlphanumeric
        };

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
        new()
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.PasswordRequiresUniqueChars, uniqueChars)
        };

    public override IdentityError PasswordRequiresUpper() =>
        new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = Messages.PasswordRequiresUpper
        };

    public override IdentityError PasswordTooShort(int length) =>
        new()
        {
            Code = nameof(PasswordTooShort),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.PasswordTooShort, length)
        };

    public override IdentityError UserAlreadyHasPassword() =>
        new()
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = Messages.UserAlreadyHasPassword
        };

    public override IdentityError UserAlreadyInRole(string role) =>
        new()
        {
            Code = nameof(UserAlreadyInRole),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.UserAlreadyInRole, role)
        };

    public override IdentityError UserNotInRole(string role) =>
        new()
        {
            Code = nameof(UserNotInRole),
            Description = string.Format(CultureInfo.InvariantCulture, Messages.UserNotInRole, role)
        };

    public override IdentityError UserLockoutNotEnabled() =>
        new()
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = Messages.UserLockoutNotEnabled
        };

    public override IdentityError RecoveryCodeRedemptionFailed() =>
        new()
        {
            Code = nameof(RecoveryCodeRedemptionFailed),
            Description = Messages.RecoveryCodeRedemptionFailed
        };

    public override IdentityError ConcurrencyFailure() =>
        new()
        {
            Code = nameof(ConcurrencyFailure),
            Description = Messages.ConcurrencyFailure
        };

    public override IdentityError DefaultError() =>
        new()
        {
            Code = nameof(DefaultError),
            Description = Messages.DefaultError
        };
#pragma warning restore CA1863
}
