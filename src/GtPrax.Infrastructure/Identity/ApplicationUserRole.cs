namespace GtPrax.Infrastructure.Identity;

using System;
using System.Security.Claims;

internal sealed class ApplicationUserRole : IEquatable<ApplicationUserRole>, IEquatable<string>
{
    public string Name { get; set; }

    public ApplicationUserRole(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    public bool Equals(ApplicationUserRole? other) =>
        other is not null &&
        other.Equals(Name);

    public bool Equals(string? other) =>
        other is not null &&
        other.Equals(Name, StringComparison.Ordinal);

    public Claim ToClaim() => new(ClaimTypes.Role, Name);

    public override bool Equals(object? obj)
    {
        if (obj is ApplicationUserRole ur)
        {
            return Equals(ur);
        }
        else
        {
            return obj is string s && Equals(s);
        }
    }

    public override int GetHashCode() => Name.GetHashCode();
}
