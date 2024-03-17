namespace GtPrax.Domain.Entities;

public sealed class User
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public UserRole[]? Roles { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    public DateTimeOffset? LockoutEndDate { get; set; }
}
