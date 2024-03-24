namespace GtPrax.UI.Pages.UsersManagement;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzer bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "Admin,Manager")]
public class EditUserModel : PageModel
{
    public void OnGet()
    {
    }
}
