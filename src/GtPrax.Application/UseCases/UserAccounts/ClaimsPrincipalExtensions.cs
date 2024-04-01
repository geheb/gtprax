namespace GtPrax.Application.UseCases.UserAccounts;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string? GetId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.NameIdentifier);
}
