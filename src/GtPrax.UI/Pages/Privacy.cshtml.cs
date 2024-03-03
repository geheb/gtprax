using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtPrax.UI.Pages;

[AllowAnonymous]
public class PrivacyModel : PageModel
{
    public PrivacyModel()
    {
    }

    public void OnGet()
    {
    }
}
