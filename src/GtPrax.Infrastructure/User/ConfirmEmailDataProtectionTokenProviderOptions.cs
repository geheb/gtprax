namespace GtPrax.Infrastructure.User;

using System;
using Microsoft.AspNetCore.Identity;

internal sealed class ConfirmEmailDataProtectionTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public static readonly string ProviderName = nameof(ConfirmEmailDataProtectionTokenProviderOptions);

    public ConfirmEmailDataProtectionTokenProviderOptions()
    {
        Name = ProviderName;
        TokenLifespan = TimeSpan.FromDays(3);
    }
}
