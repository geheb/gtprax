namespace GtPrax.UI.Pages.MyAccount;

using GtPrax.UI.Attributes;
using System.ComponentModel.DataAnnotations;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GtPrax.Application.Identity;
using GtPrax.Application.UseCases.MyAccount;

[Node("E-Mail-Adresse ändern", FromPage = typeof(IndexModel))]
[Authorize]
public class ChangeEmailModel : PageModel
{
    private readonly IMediator _mediator;

    [Display(Name = "Aktuelle E-Mail-Adresse")]
    public string? CurrentEmail { get; private set; }

    [BindProperty, Display(Name = "Neue E-Mail-Adresse")]
    [RequiredField, EmailField]
    public string? NewEmail { get; set; }

    [BindProperty, Display(Name = "Aktuelles Passwort")]
    [RequiredField, PasswordLengthField]
    public string? CurrentPassword { get; set; }

    public bool IsDisabled { get; set; }

    public ChangeEmailModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken) =>
        await UpdateView(cancellationToken);

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken))
        {
            return Page();
        }

        var callbackUrl = Url.PageLink("ConfirmChangeEmail");

        var result = await _mediator.Send(new ChangeMyEmailCommand(User.GetId()!, CurrentPassword!, NewEmail!, callbackUrl!), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage("Index", new { message = 2 });
    }

    private async Task<bool> UpdateView(CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetMyUserQuery(User.GetId()!), cancellationToken);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Der Benutzer wurde nicht gefunden.");
            IsDisabled = true;
            return false;
        }

        CurrentEmail = user.Email;
        return ModelState.IsValid;
    }
}
