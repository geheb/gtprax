namespace GtPrax.Domain.Entities;

public sealed class User
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public required string[] Roles { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    public DateTimeOffset? LockoutEndDate { get; set; }
}
