namespace GtPrax.Infrastructure.Identity;

using GtPrax.Application.Identity;
using Microsoft.AspNetCore.Identity;

internal sealed class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }
}
