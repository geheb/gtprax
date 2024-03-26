namespace GtPrax.UI.Pages.MyAccount;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Identity;
using GtPrax.Application.UseCases.MyAccount;
using GtPrax.UI.Attributes;
using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Passwort ändern", FromPage = typeof(IndexModel))]
[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly IMediator _mediator;

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

    public ChangePasswordModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _mediator.Send(new ChangeMyPasswordCommand(User.GetId()!, CurrentPassword!, NewPassword!), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }
        else
        {
            return RedirectToPage(this.PageLinkName<IndexModel>(), new { message = 1 });
        }
    }
}
