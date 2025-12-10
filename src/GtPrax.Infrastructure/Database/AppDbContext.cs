namespace GtPrax.Infrastructure.Database;

using GtPrax.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

internal sealed class AppDbContext :
    IdentityDbContext<IdentityUserGuid, IdentityRoleGuid, Guid, IdentityUserClaimGuid, IdentityUserRoleGuid, IdentityUserLoginGuid, IdentityRoleClaimGuid, IdentityUserTokenGuid>
{
    private const string SqlTypeBinary16 = "binary(16)";

    public Guid GeneratePk() => Guid.CreateVersion7();

    internal DbSet<AccountNotification> AccountNotifications => Set<AccountNotification>();
    internal DbSet<Waitlist> Waitlists => Set<Waitlist>();
    internal DbSet<WaitlistPatient> WaitlistPatients => Set<WaitlistPatient>();

    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasCharSet(CharSet.Utf8Mb4, DelegationModes.ApplyToDatabases);

        BuildUser(builder);
        BuildSuperAdmin(builder);
        BuildAccountNotify(builder);
        BuildWaitlist(builder);
    }

    private void BuildWaitlist(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Waitlist>(eb =>
        {
            eb.ToTable("waitlists");
            eb.Property(e => e.Id).HasColumnType(SqlTypeBinary16).ValueGeneratedNever();
            eb.Property(e => e.Name).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<WaitlistPatient>(eb =>
        {
            eb.ToTable("waitlist_patients");
            eb.Property(e => e.Id).HasColumnType(SqlTypeBinary16).ValueGeneratedNever();
            eb.Property(e => e.Name).HasMaxLength(256).IsRequired();
            eb.Property(e => e.UserId).HasColumnType(SqlTypeBinary16);
            eb.Property(e => e.WaitlistId).HasColumnType(SqlTypeBinary16);
            eb.Property(e => e.Created).IsRequired();
            eb.Property(e => e.PhoneNumber).HasMaxLength(16);
            eb.Property(e => e.Reason).HasMaxLength(256);
            eb.Property(e => e.Doctor).HasMaxLength(256);
            eb.Property(e => e.TherapyTimes).HasColumnType("json");
            eb.Property(e => e.Remark).HasMaxLength(1024);
            eb.Property(e => e.Tags).HasColumnType("json");

            eb.HasIndex(e => e.Created);
            eb.HasIndex(e => e.Name);
            eb.HasIndex(e => e.Birthday);

            eb.HasOne(e => e.User)
                .WithMany(e => e.WaitlistPatients)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Waitlist)
                .WithMany(e => e.WaitlistPatients)
                .HasForeignKey(e => e.WaitlistId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });
    }

    private void BuildAccountNotify(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<AccountNotification>(eb =>
        {
            eb.ToTable("account_notifications");
            eb.Property(e => e.Id).HasColumnType(SqlTypeBinary16).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(SqlTypeBinary16);
            eb.Property(e => e.Type).IsRequired();

            eb.HasOne(e => e.User)
                .WithMany(e => e.AccountNotifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasIndex(e => new { e.UserId, e.Type, e.CreatedOn });
        });

    private void BuildSuperAdmin(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new RoleSeeder());

    private void BuildUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserGuid>(eb =>
        {
            eb.Property(e => e.Id).HasColumnType(SqlTypeBinary16);
            eb.Property(e => e.Name).HasMaxLength(256);

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            eb.ToTable("users");
        });

        modelBuilder.Entity<IdentityRoleGuid>(eb =>
        {
            eb.Property(e => e.Id).HasColumnType(SqlTypeBinary16);

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .IsRequired();

            eb.ToTable("roles");
        });

        modelBuilder.Entity<IdentityUserRoleGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(SqlTypeBinary16);
            eb.Property(e => e.RoleId).HasColumnType(SqlTypeBinary16);
            eb.ToTable("user_roles");
        });

        modelBuilder.Entity<IdentityUserLoginGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(SqlTypeBinary16);
            eb.ToTable("user_logins");
        });

        modelBuilder.Entity<IdentityUserTokenGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(SqlTypeBinary16);
            eb.ToTable("user_tokens");
        });

        modelBuilder.Entity<IdentityUserClaimGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(SqlTypeBinary16);
            eb.ToTable("user_claims");
        });

        modelBuilder.Entity<IdentityRoleClaimGuid>(eb =>
        {
            eb.Property(e => e.RoleId).HasColumnType(SqlTypeBinary16);
            eb.ToTable("role_claims");
        });
    }

}
