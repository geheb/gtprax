namespace GtPrax.UI.Pages.Waitlist;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Wartelisten", FromPage = typeof(Pages.IndexModel))]
[Authorize]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
