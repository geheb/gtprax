using Microsoft.AspNetCore.Identity;

namespace GtPrax.Infrastructure.Identity;

internal sealed class NoneLookupNormalizer : ILookupNormalizer
{
	public string? NormalizeEmail(string? email) => email;

	public string? NormalizeName(string? name) => name;
}