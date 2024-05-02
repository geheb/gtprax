namespace GtPrax.UI.Pages.MyAccount;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Options;
using GtPrax.Application.UseCases.UserAccount;
using GtPrax.UI.Attributes;
using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

[Node("Zwei-Faktor-Authentifizierung bearbeiten", FromPage = typeof(IndexModel))]
[Authorize]
public class EditTwoFactorModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly string _appName;

    [BindProperty, Display(Name = "6-Stelliger Code aus der Authenticator-App")]
    [RequiredField, TextLengthField(6, MinimumLength = 6)]
    public string? Code { get; set; }

    [Display(Name = "Geheimer Schlüssel")]
    public string? SecretKey { get; set; }
    public string? AuthUri { get; set; }
    public bool IsTwoFactorEnabled { get; set; }

    public bool IsDisabled { get; set; }

    public EditTwoFactorModel(IMediator mediator, IOptions<AppOptions> appOptions)
    {
        _mediator = mediator;
        _appName = appOptions.Value.HeaderTitle;
    }

    public Task OnGetAsync(CancellationToken cancellationToken) => UpdateView(cancellationToken);

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken))
        {
            return Page();
        }
        return RedirectToPage(this.PageLinkName<IndexModel>(), new { message = 3 });
    }

    private async Task<bool> UpdateView(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateUserTwoFactorCommand(User.GetId()!, _appName), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            IsDisabled = true;
            return false;
        }

        IsTwoFactorEnabled = result.Value.IsEnabled;
        SecretKey = result.Value.SecretKey;
        AuthUri = result.Value.AuthUri;

        return ModelState.IsValid;
    }
}
