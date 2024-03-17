namespace GtPrax.Infrastructure.Identity;

using System;
using System.Linq;
using GtPrax.Domain.Entities;

internal static class ApplicationUserMapper
{
    public static User MapToUser(this ApplicationUser user) =>
        new()
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            LastLoginDate = user.LastLoginDate,
            LockoutEndDate = user.IsLockoutEnabled ? user.LockoutEndDate : null,
            Roles = user.Roles.Select(r => Enum.Parse<UserRole>(r.Name)).ToArray(),
        };

    public static User[] MapToUsers(this IEnumerable<ApplicationUser> users) =>
        users.Select(u => u.MapToUser()).ToArray();
}
