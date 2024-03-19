namespace GtPrax.UI.Pages;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private readonly ILogger _logger;

    public int Code { get; set; }
    public string? Description { get; set; }

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(int code, string? returnUrl = null)
        => HandleError(code, returnUrl);

    public void OnPost(int code)
        => HandleError(code);

    private void HandleError(int code, string? returnUrl = null)
    {
        var handler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (handler?.Error is not null)
        {
            _logger.LogError(handler.Error, "Unhandled Exception occurred.");
        }

        Code = code < 1 ? 500 : code;
        Description = code switch
        {
            400 => "Die Anfrage kann nicht bearbeitet werden, da ein Fehler beim Client vorliegt.",
            403 => $"Der Zugriff auf die angeforderte Seite '{returnUrl}' wurde verweigert.",
            404 => "Die angeforderte Seite wurde nicht gefunden.",
            _ => "Es ist ein interner Server-Fehler aufgetreten."
        };
    }
}
