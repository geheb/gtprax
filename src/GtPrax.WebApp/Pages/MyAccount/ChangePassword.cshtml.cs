namespace GtPrax.WebApp.Pages.MyAccount;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Passwort ändern", FromPage = typeof(IndexModel))]
[Authorize]
public sealed class ChangePasswordModel : PageModel
{
    private readonly IUserRepository _users;

    [BindProperty, Display(Name = "Aktuelles Passwort")]
    [RequiredField, PasswordLengthField]
    public string? CurrentPassword { get; set; }

    [BindProperty, Display(Name = "Neues Passwort")]
    [RequiredField, PasswordLengthField]
    public string? NewPassword { get; set; }

    [BindProperty, Display(Name = "Neues Passwort bestätigen")]
    [RequiredField, PasswordLengthField]
    [CompareField(nameof(NewPassword))]
    public string? ConfirmNewPassword { get; set; }

    public bool IsDisabled { get; set; }

    public ChangePasswordModel(IUserRepository users)
    {
        _users = users;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _users.ChangePassword(User.GetId(), null, NewPassword!);
        if (string.IsNullOrEmpty(result.Email))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Benutzer nicht gefunden.");
            return Page();
        }

        if (result.Error != null)
        {
            result.Error.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e));
            return Page();
        }

        return RedirectToPage("Index", new { message = 1 });
    }
}
