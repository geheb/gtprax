namespace GtPrax.UI.Pages.Login;

using System.ComponentModel.DataAnnotations;
using System.Threading;
using GtPrax.Application.UseCases.Login;
using GtPrax.UI.Attributes;
using GtPrax.UI.Extensions;
using GtPrax.UI.Routing;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public class ConfirmResetPasswordModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly NodeGeneratorService _nodeGeneratorService;

    [BindProperty]
    public string? UserNameBot { get; set; }

    [BindProperty, Display(Name = "Passwort")]
    [RequiredField, PasswordLengthField]
    public string? Password { get; set; }

    [BindProperty, Display(Name = "Passwort wiederholen")]
    [RequiredField, PasswordLengthField]
    [CompareField(nameof(Password))]
    public string? RepeatPassword { get; set; }

    public bool IsDisabled { get; set; }

    public ConfirmResetPasswordModel(IMediator mediator, NodeGeneratorService nodeGeneratorService)
    {
        _mediator = mediator;
        _nodeGeneratorService = nodeGeneratorService;
    }

    public async Task OnGetAsync(string id, string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(token))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return;
        }

        var result = await _mediator.Send(new VerifyResetPasswordTokenQuery(id, token), cancellationToken);
        if (result.IsFailed)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, Messages.InvalidResetPasswordToken);
            return;
        }
    }

    public async Task<IActionResult> OnPostAsync(string id, string token, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(UserNameBot) || string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(token))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return Page();
        }

        var result = await _mediator.Send(new ConfirmResetPasswordCommand(id, token, Password!), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }
        else
        {
            return RedirectToPage(_nodeGeneratorService.GetNode<IndexModel>().Page, new { message = 2 });
        }
    }

}
