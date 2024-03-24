namespace GtPrax.UI.Pages;

using GtPrax.UI.Extensions;
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
        Code = code < 1 ? 500 : code;
        Description = code switch
        {
            400 => Messages.ProcessRequestFailed,
            403 => Messages.PageAccessDenied(returnUrl),
            404 => Messages.PageNotFound,
            _ => Messages.InternalServerError
        };
    }
}
