namespace GtPrax.Infrastructure.User;

using System.Linq;
using System.Security.Claims;
using GtPrax.Domain.Models;
using GtPrax.Domain.ValueObjects;

internal static class UserMapping
{
    public static User MapToDomain(this UserModel user) =>
        new(Id: user.Id.ToString(),
            Name: user.Name,
            Email: user.Email,
            Roles: UserRoleType.From(user.Claims.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Select(r => r.Value)),
            IsEmailConfirmed: user.IsEmailConfirmed,
            LastLoginDate: user.LastLoginDate,
            LockoutEndDate: user.IsLockoutEnabled ? user.LockoutEndDate : null);

    public static User[] MapToDomain(this IEnumerable<UserModel> users) =>
        users.Select(u => u.MapToDomain()).ToArray();
}
