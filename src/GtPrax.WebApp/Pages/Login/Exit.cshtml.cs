namespace GtPrax.WebApp.Pages.Login;

using GtPrax.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public sealed class ExitModel : PageModel
{
    private readonly IUserRepository _users;

    public ExitModel(IUserRepository users)
    {
        _users = users;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _users.SignOutCurrentUser();

        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        return LocalRedirect("/");
    }
}
