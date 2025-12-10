namespace GtPrax.WebApp.Pages.MyAccount;

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("E-Mail-Adresse ändern", FromPage = typeof(IndexModel))]
[Authorize]
public sealed class ChangeEmailModel : PageModel
{
    private readonly IUserRepository _users;

    [Display(Name = "Aktuelle E-Mail-Adresse")]
    public string? CurrentEmail { get; private set; }

    [BindProperty, Display(Name = "Neue E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? NewEmail { get; set; }

    [BindProperty, Display(Name = "Aktuelles Passwort")]
    [RequiredField, PasswordLengthField]
    public string? CurrentPassword { get; set; }

    public bool IsDisabled { get; set; }

    public ChangeEmailModel(IUserRepository users)
    {
        _users = users;
    }

    public void OnGet() =>
        CurrentEmail = User.FindFirstValue(ClaimTypes.Email);

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        CurrentEmail = User.FindFirstValue(ClaimTypes.Email);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _users.NotifyConfirmChangeEmail(User.GetId(), NewEmail!, CurrentPassword!, cancellationToken);
        if (!string.IsNullOrEmpty(result.Error))
        {
            ModelState.AddModelError(string.Empty, result.Error);
            IsDisabled = result.IsFatal;
            return Page();
        }

        return RedirectToPage("Index", new { message = 2 });
    }
}
