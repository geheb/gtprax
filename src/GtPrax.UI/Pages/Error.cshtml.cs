namespace GtPrax.UI.Pages;

using GtPrax.UI.Extensions;
using GtPrax.UI.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private readonly NodeGeneratorService _nodeGeneratorService;

    public int Code { get; set; }
    public string? Description { get; set; }
    public bool Is2faRequired { get; set; }

    public ErrorModel(NodeGeneratorService nodeGeneratorService)
    {
        _nodeGeneratorService = nodeGeneratorService;
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

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            var node = _nodeGeneratorService.Find(returnUrl);
            Is2faRequired = node?.AllowedPolicy == Policies.Require2fa;
        }
    }
}
