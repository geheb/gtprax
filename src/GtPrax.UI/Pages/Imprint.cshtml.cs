using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtPrax.UI.Pages;

[AllowAnonymous]
public class ImprintModel : PageModel
{
    public void OnGet()
    {
    }
}
