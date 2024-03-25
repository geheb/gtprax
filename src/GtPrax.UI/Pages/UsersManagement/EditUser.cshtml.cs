namespace GtPrax.UI.Pages.UsersManagement;

using GtPrax.Application.UseCases.UsersManagement;
using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzer bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "Admin,Manager")]
public class EditUserModel : PageModel
{
    private readonly IMediator _mediator;

    [BindProperty]
    public EditUserInput Input { get; set; } = new();

    public UserDto? Info { get; set; }

    public bool IsDisabled { get; set; }

    public EditUserModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(string id, CancellationToken cancellationToken)
    {
        await UpdateView(id, cancellationToken);
        if (Info is not null)
        {
            Input.Set(Info);
        }
    }

    public async Task<IActionResult> OnPostAsync(string id, CancellationToken cancellationToken)
    {
        if (!await UpdateView(id, cancellationToken))
        {
            return Page();
        }

        var roles = Input.GetRoles();
        if (roles.Length < 1)
        {
            ModelState.AddModelError(string.Empty, Messages.RoleRequired);
            return Page();
        }

        var result = await _mediator.Send(new UpdateUserCommand(id, Input.Name, Input.Email, Input.Password, roles), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage(StringExtensions.PageLinkName<IndexModel>());
    }

    private async Task<bool> UpdateView(string id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserQuery(id), cancellationToken);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, Messages.UserNotFound);
            IsDisabled = true;
            return false;
        }

        Info = user;
        return ModelState.IsValid;
    }
}
