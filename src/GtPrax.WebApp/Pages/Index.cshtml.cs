namespace GtPrax.WebApp.Pages;

using GtPrax.Infrastructure.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Startseite", IsDefault = true)]
[AllowAnonymous]
public sealed class IndexModel : PageModel
{
}
