namespace GtPrax.Domain.Entities;

using GtPrax.Domain.ValueObjects;

public sealed record User(string Id, string Name, string Email, UserRoleType[] Roles, bool IsEmailConfirmed, DateTimeOffset? LastLoginDate, DateTimeOffset? LockoutEndDate);
