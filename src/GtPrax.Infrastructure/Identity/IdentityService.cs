namespace GtPrax.Infrastructure.Identity;

using System.Threading.Tasks;
using GtPrax.Application.Identity;
using Microsoft.AspNetCore.Identity;

internal sealed class IdentityService : IIdentityService
{
    private readonly TimeProvider _timeProvider;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        TimeProvider timeProvider,
        SignInManager<ApplicationUser> signInManager)
    {
        _timeProvider = timeProvider;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> SignIn(string email, string password)
    {
        var userManager = _signInManager.UserManager;

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "LoginFailed", Description = "Die Anmeldung ist fehlgeschlagen." });
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            user.LastLogin = _timeProvider.GetUtcNow();
            await _signInManager.UserManager.UpdateAsync(user);
            return IdentityResult.Success;
        }
        else if (result.IsLockedOut)
        {
            return IdentityResult.Failed(new IdentityError { Code = "LockedOut", Description = "Dein Login ist vorübergehend gesperrt." });
        }
        else if (result.IsNotAllowed)
        {
            return IdentityResult.Failed(new IdentityError { Code = "NotActivated", Description = "Dein Login ist nicht freigeschaltet." });
        }
        else
        {
            return IdentityResult.Failed(new IdentityError { Code = "LoginFailed", Description = "Die Anmeldung ist fehlgeschlagen." });
        }
    }

    public Task SignOutCurrentUser() => _signInManager.SignOutAsync();
}
