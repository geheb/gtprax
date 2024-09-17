namespace GtPrax.UI.Pages.Login;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.UseCases.Login;
using GtPrax.UI.Attributes;
using GtPrax.UI.Extensions;
using GtPrax.UI.Routing;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public class ResetPasswordModel : PageModel
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;
    private readonly NodeGeneratorService _nodeGeneratorService;

    [BindProperty]
    public string? UserNameBot { get; set; }

    [BindProperty, Display(Name = "Deine E-Mail-Adresse")]
    [RequiredField, EmailField]
    public string? Email { get; set; }

    public bool IsDisabled { get; set; }

    public ResetPasswordModel(
        ILogger<ResetPasswordModel> logger,
        IMediator mediator,
        NodeGeneratorService nodeGeneratorService)
    {
        _logger = logger;
        _mediator = mediator;
        _nodeGeneratorService = nodeGeneratorService;
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

        var callbackUrl = Url.PageLink(_nodeGeneratorService.GetNode<ConfirmResetPasswordModel>().Page);
        var result = await _mediator.Send(new ResetPasswordCommand(Email!, callbackUrl!), cancellationToken);
        if (result.IsFailed)
        {
            _logger.ResetPasswordFailed(Email!.AnonymizeEmail(), result.Errors);
        }

        return RedirectToPage(_nodeGeneratorService.GetNode<IndexModel>().Page, new { message = 1 });
    }
}
