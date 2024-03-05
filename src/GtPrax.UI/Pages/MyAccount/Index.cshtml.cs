namespace GtPrax.UI.Pages.MyAccount;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Mein Konto", FromPage = typeof(Pages.IndexModel))]
[Authorize]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
