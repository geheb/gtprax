using System.Security.Claims;

namespace GtPrax.Infrastructure.Identity;

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

	public Claim ToClaim() => new Claim(Type, Value);

	public bool Equals(ApplicationUserClaim? other) =>
		other is not null &&
		other.Type.Equals(Type) &&
		other.Value.Equals(Value);

	public bool Equals(Claim? other) =>
		other is not null &&
		other.Type.Equals(Type) &&
		other.Value.Equals(Value);

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
