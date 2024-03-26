namespace GtPrax.UI.Pages.UsersManagement;

using GtPrax.Application.UseCases.UsersManagement;
using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using GtPrax.UI.Pages.Login;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzer anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "Admin,Manager")]
public class CreateUserModel : PageModel
{
    private readonly IMediator _mediator;

    [BindProperty]
    public CreateUserInput Input { get; set; } = new();

    public CreateUserModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var roles = Input.GetRoles();
        if (roles.Length < 1)
        {
            ModelState.AddModelError(string.Empty, Messages.RoleRequired);
            return Page();
        }

        var callbackUrl = Url.PageLink(this.PageLinkName<ConfirmEmailModel>());

        var result = await _mediator.Send(new CreateUserCommand(Input.Name!, Input.Email!, roles, callbackUrl!), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage(this.PageLinkName<IndexModel>());
    }
}
