namespace GtPrax.Infrastructure.User;

using System.Linq;
using System.Security.Claims;
using GtPrax.Domain.Models;
using GtPrax.Domain.ValueObjects;

internal static class UserMapping
{
    public static User MapToDomain(this UserModel user) =>
        new(user.Id.ToString(),
            user.Name,
            user.Email,
            UserRoleType.From(user.Claims.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Select(r => r.Value)),
            user.IsEmailConfirmed,
            user.LastLoginDate,
            user.IsLockoutEnabled ? user.LockoutEndDate : null);

    public static User[] MapToDomain(this IEnumerable<UserModel> users) =>
        users.Select(u => u.MapToDomain()).ToArray();
}
