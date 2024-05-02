namespace GtPrax.Application.UseCases.UserAccount;

using GtPrax.Application.Converter;
using GtPrax.Domain.Models;

internal static class UserMappings
{
    public static UserDto MapToDto(this User item, GermanDateTimeConverter dateTimeConverter, TimeProvider timeProvider) =>
        new(Id: item.Id,
            Name: item.Name,
            Email: item.Email,
            IsEmailConfirmed: item.IsEmailConfirmed,
            LastLogin: item.LastLoginDate is not null ? dateTimeConverter.ToLocal(item.LastLoginDate.Value) : null,
            LockoutEnd: item.LockoutEndDate is not null ? dateTimeConverter.ToLocal(item.LockoutEndDate.Value) : null,
            IsLockout: item.LockoutEndDate > timeProvider.GetUtcNow(),
            Roles: item.Roles.Select(r => (UserRole)r.Key).ToArray());

    public static UserTwoFactorDto MapToDto(this UserTwoFactor item) =>
        new(item.IsEnabled, item.SecretKey, item.AuthUri);
}
