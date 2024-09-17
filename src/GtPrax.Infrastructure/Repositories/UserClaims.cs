namespace GtPrax.Infrastructure.Repositories;

using System.Security.Claims;

internal static class UserClaims
{
    // Authentication Method Reference (amr)
    public static readonly Claim TwoFactorClaim = new("amr", "mfa");
}
