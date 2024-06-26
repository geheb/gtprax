namespace GtPrax.UI.Pages.MyAccount;

using System.ComponentModel.DataAnnotations;
using System.Threading;
using GtPrax.Application.UseCases.UserAccount;
using GtPrax.UI.Attributes;
using GtPrax.UI.Extensions;
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
            0 => Messages.ChangesSaved,
            1 => Messages.PasswordSaved,
            2 => Messages.ChangeEmailSent,
            3 => Messages.TwoFactorEnabled,
            4 => Messages.TwoFactorDisabled,
            _ => default
        };

    private async Task<UserDto?> UpdateView(CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new FindUserByIdQuery(User.GetId()!), cancellationToken);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, Messages.UserNotFound);
            IsDisabled = true;
            return null;
        }

        Email = user.Email;
        return ModelState.IsValid ? user : null;
    }
}
