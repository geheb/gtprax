namespace GtPrax.WebApp.Pages.Login;

using GtPrax.WebApp.I18n;
using GtPrax.Infrastructure.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Repositories;

[AllowAnonymous]
public sealed class IndexModel : PageModel
{
    private readonly IUserRepository _users;

    [BindProperty]
    public string? UserName { get; set; }

    [BindProperty, Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [BindProperty, Display(Name = "Passwort")]
    [RequiredField, PasswordLengthField]
    public string? Password { get; set; }

    public string? Message { get; set; }

    public IndexModel(
        IUserRepository users)
    {
        _users = users;
    }

    public void OnGet(int message = 0)
    {
        if (message == 1)
        {
            Message = "Das Passwort wurde geändert. Melde dich jetzt mit dem neuen Passwort an.";
        }
        else if (message == 2)
        {
            Message = "Eine E-Mail wird an die E-Mail-Adresse versendet, um das Passwort zu ändern.";
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(UserName))
        {
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var error = await _users.SignIn(Email!, Password!);
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        return LocalRedirect(returnUrl ?? "/");
    }
}
