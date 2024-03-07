namespace GtPrax.UI.Pages.Login;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Identity;
using GtPrax.UI.Attributes;
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

    public IndexModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(UserName))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Die Anfrage ist ungültig.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _identityService.SignIn(Email!, Password!);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Die Anmeldung ist fehlgeschlagen.");
            return Page();
        }

        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }
}
