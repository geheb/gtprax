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
public class IndexModel : PageModel
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    [BindProperty]
    public string? UserNameBot { get; set; }

    [BindProperty, Display(Name = "E-Mail-Adresse"), RequiredField, EmailField]
    public string? Email { get; set; }

    [BindProperty, Display(Name = "Passwort"), RequiredField, PasswordLengthField]
    public string? Password { get; set; }

    public bool IsDisabled { get; set; }
    public string? Message { get; set; }

    public IndexModel(
        ILogger<IndexModel> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public void OnGet(int? message)
    {
        if (message == 1)
        {
            Message = Messages.ResetPasswordSent;
        }
        else if (message == 2)
        {
            Message = Messages.PasswordChanged;
        }
        else if (message == 3)
        {
            Message = Messages.EmailConfirmed;
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl, CancellationToken cancellationToken)
    {
        Message = null;

        if (!string.IsNullOrWhiteSpace(UserNameBot))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, Messages.InvalidRequest);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _mediator.Send(new SignInCommand(Email!, Password!), cancellationToken);
        if (result.IsFailed)
        {
            _logger.SignInFailed(Email!.AnonymizeEmail(), result.Errors);
            ModelState.AddModelError(string.Empty, Messages.SignInFailed);
            return Page();
        }

        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }
}
