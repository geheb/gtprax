namespace GtPrax.Application.UseCases.UserAccounts;

public sealed record UserDto(string Id, string Name, string Email, bool IsEmailConfirmed, DateTimeOffset? LastLogin, DateTimeOffset? LockoutEnd, bool IsLockout, UserRole[] Roles);
