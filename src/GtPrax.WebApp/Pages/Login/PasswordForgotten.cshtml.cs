namespace GtPrax.WebApp.Pages.Login;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Repositories;
using GtPrax.Application.Services;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.WebApp.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public sealed class PasswordForgottenModel : PageModel
{
    private readonly IUserRepository _users;
    private readonly IEmailValidator _emailValidator;

    [BindProperty]
    public string? UserName { get; set; } // just for Bots

    [BindProperty, Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [BindProperty]
    public bool IsDisabled { get; set; }

    public PasswordForgottenModel(
        IUserRepository users,
        IEmailValidator emailValidator)
    {
        _users = users;
        _emailValidator = emailValidator;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(UserName))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!await _emailValidator.Validate(Email!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, Messages.InvalidEmail);
            return Page();
        }

        if (!await _users.NotifyPasswordForgotten(Email!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, Messages.SaveFailed);
            return Page();
        }

        return RedirectToPage("Index", new { message = 2 });
    }
}
