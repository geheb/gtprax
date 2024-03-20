namespace GtPrax.UI.Pages.MyAccount;

using System.ComponentModel.DataAnnotations;
using System.Threading;
using GtPrax.Application.Identity;
using GtPrax.Application.UseCases.MyAccount;
using GtPrax.UI.Attributes;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Mein Konto", FromPage = typeof(Pages.IndexModel))]
[Authorize]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    [Display(Name = "E-Mail-Adresse")]
    public string? Email { get; set; }

    [BindProperty, Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    public bool IsDisabled { get; set; }

    public string? Message { get; set; }

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(int? message, CancellationToken cancellationToken)
    {
        SetMessage(message);

        var user = await UpdateView(cancellationToken);
        Name = user?.Name;
    }

    public async Task OnPostAsync(CancellationToken cancellationToken)
    {
        if (await UpdateView(cancellationToken) is null)
        {
            return;
        }

        var result = await _mediator.Send(new UpdateMyUserCommand(User.GetId()!, Name!), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
        }
        else
        {
            SetMessage(0);
        }
    }

    private void SetMessage(int? message) =>
        Message = message switch
        {
            0 => "Änderungen wurden gespeichert.",
            1 => "Das Passwort wurde geändert.",
            2 => "Eine E-Mail wird an die neue E-Mail-Adresse versendet. Diese muss bestätigt werden - erst dann ist die Änderung vollständig.",
            _ => default
        };

    private async Task<MyUserDto?> UpdateView(CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetMyUserQuery(User.GetId()!), cancellationToken);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Der Benutzer wurde nicht gefunden.");
            IsDisabled = true;
            return null;
        }

        Email = user.Email;
        return ModelState.IsValid ? user : null;
    }
}
