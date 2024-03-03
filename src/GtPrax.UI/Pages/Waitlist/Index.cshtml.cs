using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtPrax.UI.Pages.Waitlist;

[Node("Wartelisten", FromPage = typeof(Pages.IndexModel))]
[Authorize]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
