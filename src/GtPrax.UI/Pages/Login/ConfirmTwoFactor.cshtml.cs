namespace GtPrax.UI.Pages.Login;

using GtPrax.UI.Attributes;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mediator;
using GtPrax.Application.UseCases.Login;
using GtPrax.UI.Extensions;

public class ConfirmTwoFactorModel : PageModel
{
    private readonly IMediator _mediator;

    [BindProperty]
    public string? UserNameBot { get; set; }

    [BindProperty, Display(Name = "6-stelliger Code aus der Authenticator-App")]
    [RequiredField, TextLengthField(6, MinimumLength = 6)]
    public string? Code { get; set; }

    [BindProperty, Display(Name = "Diesen Browser vertrauen")]
    public bool IsTrustBrowser { get; set; }

    public string? ReturnUrl { get; set; }

    public bool IsDisabled { get; set; }

    public ConfirmTwoFactorModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void OnGet(string? returnUrl) => ReturnUrl = returnUrl;

    public async Task<IActionResult> OnPostAsync(string? returnUrl, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(UserNameBot))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return Page();
        }

        var result = await _mediator.Send(new SignInTwoFactorCommand(Code!, IsTrustBrowser), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }
}
