namespace GtPrax.UI.Pages;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Startseite", IsDefault = true)]
[AllowAnonymous]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
