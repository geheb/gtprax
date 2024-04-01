namespace GtPrax.Application.UseCases.UserAccounts;

using GtPrax.Application.Converter;
using GtPrax.Domain.Entities;

internal static class UserMappings
{
    public static UserDto MapToDto(this User user, GermanDateTimeConverter dateTimeConverter, TimeProvider timeProvider) =>
        new(Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            IsEmailConfirmed: user.IsEmailConfirmed,
            LastLogin: user.LastLoginDate is not null ? dateTimeConverter.ToLocal(user.LastLoginDate.Value) : null,
            LockoutEnd: user.LockoutEndDate is not null ? dateTimeConverter.ToLocal(user.LockoutEndDate.Value) : null,
            IsLockout: user.LockoutEndDate > timeProvider.GetUtcNow(),
            Roles: user.Roles.Select(r => (UserRole)r.Key).ToArray());
}
