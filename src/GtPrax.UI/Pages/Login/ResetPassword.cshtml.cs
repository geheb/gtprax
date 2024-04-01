namespace GtPrax.UI.Pages.Login;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.UseCases.Login;
using GtPrax.UI.Attributes;
using GtPrax.UI.Extensions;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public class ResetPasswordModel : PageModel
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    [BindProperty]
    public string? UserNameBot { get; set; }

    [BindProperty, Display(Name = "Deine E-Mail-Adresse")]
    [RequiredField, EmailField]
    public string? Email { get; set; }

    public bool IsDisabled { get; set; }

    public ResetPasswordModel(
        ILogger<ResetPasswordModel> logger,
        IMediator mediator)
    {
        _logger = logger;
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

        if (!string.IsNullOrWhiteSpace(UserNameBot))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return Page();
        }

        var callbackUrl = Url.PageLink(this.PageLinkName<ConfirmResetPasswordModel>());
        var result = await _mediator.Send(new ResetPasswordCommand(Email!, callbackUrl!), cancellationToken);
        if (result.IsFailed)
        {
            _logger.ResetPasswordFailed(Email!.AnonymizeEmail(), result.Errors);
        }

        return RedirectToPage(this.PageLinkName<IndexModel>(), new { message = 1 });
    }
}
