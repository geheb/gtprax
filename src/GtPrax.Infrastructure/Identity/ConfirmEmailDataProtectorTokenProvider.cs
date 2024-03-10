namespace GtPrax.Infrastructure.Identity;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class ConfirmEmailDataProtectorTokenProvider : DataProtectorTokenProvider<ApplicationUser>
{
    public ConfirmEmailDataProtectorTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> options,
        ILogger<ConfirmEmailDataProtectorTokenProvider> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}
