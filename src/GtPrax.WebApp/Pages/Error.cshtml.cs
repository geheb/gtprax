namespace GtPrax.WebApp.Pages;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[AllowAnonymous]
[IgnoreAntiforgeryToken]
public sealed class ErrorModel : PageModel
{
    public int ErrorCode { get; set; } = 500;
    public string? ErrorDescription { get; set; }

    public void OnGet(int statusCode, string? returnUrl = null)
    {
        if (statusCode > 0)
        {
            ErrorCode = statusCode;
        }

        ErrorDescription = statusCode switch
        {
            400 => "Die Anfrage kann nicht bearbeitet werden, da ein Fehler beim Client vorliegt.",
            403 => $"Der Zugriff auf die angeforderte Seite '{returnUrl}' wurde verweigert.",
            404 => "Die angeforderte Seite wurde nicht gefunden.",
            _ => "Es ist ein interner Server-Fehler aufgetreten."
        };
    }
}
