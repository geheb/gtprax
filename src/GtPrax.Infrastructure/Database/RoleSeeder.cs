namespace GtPrax.Infrastructure.Database;

using GtPrax.Application.Models;
using GtPrax.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
internal sealed class RoleSeeder : IEntityTypeConfiguration<IdentityRoleGuid>
{
    public void Configure(EntityTypeBuilder<IdentityRoleGuid> builder) =>
        builder.HasData(
            new IdentityRoleGuid
            {
                Id = Guid.Parse("FBBCF7BF-8CBF-440F-939C-F67631109AA0"),
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpperInvariant(),
                ConcurrencyStamp = "CFAD6F62-EEAA-4ECD-B847-0762C704EC45"
            },
            new IdentityRoleGuid
            {
                Id = Guid.Parse("1FB64576-3F90-4CE9-8C3A-CFA13F587167"),
                Name = Roles.Manager,
                NormalizedName = Roles.Manager.ToUpperInvariant(),
                ConcurrencyStamp = "6EE38FDC-5CE5-42FD-A5A5-168573DB2F86"
            },
            new IdentityRoleGuid
            {
                Id = Guid.Parse("03A90DA5-ABF0-45E9-9B80-CDAC790B7105"),
                Name = Roles.Staff,
                NormalizedName = Roles.Staff.ToUpperInvariant(),
                ConcurrencyStamp = "956824FE-2F13-4919-B2D2-0E60BECFCA12"
            }
        );
}
