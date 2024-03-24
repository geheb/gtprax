namespace GtPrax.UI.Pages.UsersManagement;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzer anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "Admin,Manager")]
public class CreateUserModel : PageModel
{
    public void OnGet()
    {
    }
}
