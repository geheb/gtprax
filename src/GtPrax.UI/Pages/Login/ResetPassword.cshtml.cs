namespace GtPrax.UI.Pages.Login;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.UseCases.Login;
using GtPrax.UI.Attributes;
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
    public string? UserName { get; set; }

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
        if (!string.IsNullOrWhiteSpace(UserName))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Die Anfrage ist ungültig.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var callbackUrl = Url.PageLink("ConfirmResetPassword");
        var result = await _mediator.Send(new ResetPasswordCommand(Email!, callbackUrl!), cancellationToken);
        if (result.IsFailed)
        {
            _logger.LogWarning("Request reset password for user {Email} failed: {Error}", Email, string.Join(", ", result.Errors.Select(e => e.Message)));
        }

        return RedirectToPage("Index", new { message = 1 });
    }
}
