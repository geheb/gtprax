namespace GtPrax.Infrastructure.Identity;

using System.Linq;
using System.Security.Claims;
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
            IsEmailConfirmed = user.IsEmailConfirmed,
            Roles = user.Claims.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Select(r => r.Value).ToArray(),
        };

    public static User[] MapToUsers(this IEnumerable<ApplicationUser> users) =>
        users.Select(u => u.MapToUser()).ToArray();
}
