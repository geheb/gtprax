namespace GtPrax.UI.Pages.UserAccount;

using GtPrax.Application.UseCases.UserAccount;
using GtPrax.UI.Models;
using GtPrax.UI.Routing;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzerverwaltung", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "Admin,Manager", Policy = Policies.Require2fa)]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public UserDto[] Items { get; set; } = [];
    public int UsersConfirmed { get; set; }
    public int UsersNotConfirmed { get; set; }
    public int UsersLocked { get; set; }

    public IndexModel(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
        UsersConfirmed = Items.Count(u => u.IsEmailConfirmed);
        UsersNotConfirmed = Items.Count(u => !u.IsEmailConfirmed);
        UsersLocked = Items.Count(u => u.IsLockout);
    }
}
