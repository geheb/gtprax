namespace GtPrax.Infrastructure.Database.Entities;

using GtPrax.Application.Converter;
using GtPrax.Application.Models;
using Microsoft.AspNetCore.Identity;

internal sealed class IdentityUserGuid : IdentityUser<Guid>
{
    public string? Name { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    public DateTimeOffset? RemovedOn { get; set; }

    internal ICollection<IdentityUserRoleGuid>? UserRoles { get; set; }
    internal ICollection<AccountNotification>? AccountNotifications { get; set; }
    internal ICollection<WaitlistPatient>? WaitlistPatients { get; set; }

    public UserDto ToDto(EmailConverter ec, GermanDateTimeConverter dc)
    {
        var roles = UserRoles?.Where(e => e.Role != null).Select(e => e.Role!.Name!).ToArray();
        return new()
        {
            Id = Id,
            Name = Name,
            Email = ec.Normalize(Email!),
            EmailConfirmed = EmailConfirmed,
            LastLogin = LastLogin.HasValue ? dc.ToLocal(LastLogin.Value) : null,
            Roles = roles,
            CanDelete = roles != null && !roles.Any(r => r == Roles.Admin),
            IsEmailConfirmed = EmailConfirmed,
            IsLocked = LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow,
        };
    }
}
