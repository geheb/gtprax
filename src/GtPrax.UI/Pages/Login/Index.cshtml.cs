namespace GtPrax.UI.Pages.Login;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Identity;
using GtPrax.UI.Attributes;
using GtPrax.UI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly IIdentityService _identityService;

    [BindProperty]
    public string? UserName { get; set; }

    [BindProperty, Display(Name = "E-Mail-Adresse"), RequiredField, EmailField]
    public string? Email { get; set; }

    [BindProperty, Display(Name = "Passwort"), RequiredField, PasswordLengthField]
    public string? Password { get; set; }

    public bool IsDisabled { get; set; }
    public string? Message { get; set; }

    public IndexModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public void OnGet(int? message)
    {
        if (message == 1)
        {
            Message = Messages.ResetPasswordSent;
        }
        else if (message == 2)
        {
            Message = Messages.PasswordChanged;
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl, CancellationToken cancellationToken)
    {
        Message = null;

        if (!string.IsNullOrWhiteSpace(UserName))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _identityService.SignIn(Email!, Password!, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, Messages.LoginFailed);
            return Page();
        }

        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }
}
