namespace GtPrax.UI.Pages.UsersManagement;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzer anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "Admin,Manager")]
public class CreateUserModel : PageModel
{
    [BindProperty]
    public CreateUserInput Input { get; set; } = new();

    public void OnGet()
    {
    }
}
