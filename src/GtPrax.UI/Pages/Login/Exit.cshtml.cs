namespace GtPrax.UI.Pages.Login;

using GtPrax.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class ExitModel : PageModel
{
    private readonly IIdentityService _identityService;

    public ExitModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _identityService.SignOutCurrentUser();

        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        return LocalRedirect(Url.Content("~/"));
    }
}
