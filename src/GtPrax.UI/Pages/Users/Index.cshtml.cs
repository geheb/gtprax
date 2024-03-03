using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtPrax.UI.Pages.Users;

[Node("Benutzerverwaltung", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "admin,manager")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
