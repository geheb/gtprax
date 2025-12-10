namespace GtPrax.Application.Models;

public sealed class UserDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    public string[]? Roles { get; set; }
    public bool CanDelete { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsLocked { get; set; }
}
