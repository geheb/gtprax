namespace GtPrax.Infrastructure.Identity;

using System.Security.Claims;

internal sealed class ApplicationUserClaim : IEquatable<ApplicationUserClaim>, IEquatable<Claim>
{
    public ApplicationUserClaim(Claim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        Type = claim.Type;
        Value = claim.Value;
    }

    public ApplicationUserClaim(string claimType, string claimValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimValue);
        Type = claimType;
        Value = claimValue;
    }

    public string Type { get; set; }

    public string Value { get; set; }

    public Claim ToClaim() => new(Type, Value);

    public bool Equals(ApplicationUserClaim? other) =>
        other is not null &&
        other.Type.Equals(Type, StringComparison.Ordinal) &&
        other.Value.Equals(Value, StringComparison.Ordinal);

    public bool Equals(Claim? other) =>
        other is not null &&
        other.Type.Equals(Type, StringComparison.Ordinal) &&
        other.Value.Equals(Value, StringComparison.Ordinal);

    public override bool Equals(object? obj)
    {
        if (obj is ApplicationUserClaim uc)
        {
            return Equals(uc);
        }
        else
        {
            return obj is Claim c && Equals(c);
        }
    }

    public override int GetHashCode() => Type.GetHashCode() ^ Value.GetHashCode();
}
