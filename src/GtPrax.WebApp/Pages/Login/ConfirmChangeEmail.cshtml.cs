namespace GtPrax.WebApp.Pages.Login;

using GtPrax.Application.Repositories;
using GtPrax.WebApp.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GtPrax.Application.Converter;

[AllowAnonymous]
public sealed class ConfirmChangeEmailModel : PageModel
{
    private readonly IUserRepository _users;

    public string? ConfirmedEmail { get; set; }

    public ConfirmChangeEmailModel(IUserRepository users)
    {
        _users = users;
    }

    public async Task OnGetAsync(Guid id, string token, string email)
    {
        if (id == Guid.Empty || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return;
        }

        var newEmail = await _users.ConfirmChangeEmail(id, token, email);
        if (string.IsNullOrEmpty(newEmail))
        {
            ModelState.AddModelError(string.Empty, Messages.InvalidNewEmailConfirmationLink);
            return;
        }

        ConfirmedEmail = new EmailConverter().Anonymize(newEmail);
    }
}
