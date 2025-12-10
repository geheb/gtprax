namespace GtPrax.Infrastructure.Database.Entities;

using Microsoft.AspNetCore.Identity;

internal sealed class IdentityUserRoleGuid : IdentityUserRole<Guid>
{
    public IdentityUserGuid? User { get; set; }
    public IdentityRoleGuid? Role { get; set; }
}
