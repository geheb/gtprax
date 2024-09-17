namespace GtPrax.UI.Pages.UserAccount;

using GtPrax.Application.UseCases.UserAccount;
using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using GtPrax.UI.Pages.Login;
using GtPrax.UI.Routing;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzer anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "Admin,Manager")]
public class CreateModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly NodeGeneratorService _nodeGeneratorService;

    [BindProperty]
    public CreateInput Input { get; set; } = new();

    public CreateModel(IMediator mediator, NodeGeneratorService nodeGeneratorService)
    {
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

        var roles = Input.GetRoles();
        if (roles.Length < 1)
        {
            ModelState.AddModelError(string.Empty, Messages.RoleRequired);
            return Page();
        }

        var callbackUrl = Url.PageLink(_nodeGeneratorService.GetNode<ConfirmEmailModel>().Page);

        var result = await _mediator.Send(new CreateUserCommand(Input.Name!, Input.Email!, roles, callbackUrl!), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage(_nodeGeneratorService.GetNode<IndexModel>().Page);
    }
}
