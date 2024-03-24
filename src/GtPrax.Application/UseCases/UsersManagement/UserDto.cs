namespace GtPrax.Application.UseCases.UsersManagement;

using GtPrax.Application.Identity;

public sealed record UserDto(string Id, string Name, string Email, bool IsEmailConfirmed, DateTimeOffset? LastLogin, DateTimeOffset? LockoutEnd, bool IsLockout, UserRole[] Roles);
