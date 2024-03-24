namespace GtPrax.UI.Pages.MyAccount;

using GtPrax.Application.UseCases.MyAccount;
using GtPrax.UI.Extensions;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public class ConfirmChangeEmailModel : PageModel
{
    private readonly IMediator _mediator;

    public string? NewEmail { get; set; }

    public ConfirmChangeEmailModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(string id, string token, string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return;
        }

        NewEmail = email.AnonymizeEmail();

        var result = await _mediator.Send(new ConfirmChangeMyEmailCommand(id, token, email), cancellationToken);
        if (!result.IsSuccess)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return;
        }
    }
}
