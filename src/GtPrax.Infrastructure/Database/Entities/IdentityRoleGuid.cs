namespace GtPrax.Infrastructure.Database.Entities;

using Microsoft.AspNetCore.Identity;

internal sealed class IdentityRoleGuid : IdentityRole<Guid>
{
    public ICollection<IdentityUserRoleGuid>? UserRoles { get; set; }
}
