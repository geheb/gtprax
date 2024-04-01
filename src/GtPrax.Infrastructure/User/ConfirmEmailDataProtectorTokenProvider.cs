namespace GtPrax.Infrastructure.User;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class ConfirmEmailDataProtectorTokenProvider : DataProtectorTokenProvider<UserModel>
{
    public ConfirmEmailDataProtectorTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> options,
        ILogger<ConfirmEmailDataProtectorTokenProvider> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}
