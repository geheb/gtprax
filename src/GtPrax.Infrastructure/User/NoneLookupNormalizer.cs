namespace GtPrax.Infrastructure.User;

using Microsoft.AspNetCore.Identity;

internal sealed class NoneLookupNormalizer : ILookupNormalizer
{
    public string? NormalizeEmail(string? email) => email;

    public string? NormalizeName(string? name) => name;
}
