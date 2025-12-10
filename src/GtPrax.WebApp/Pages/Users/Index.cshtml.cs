namespace GtPrax.WebApp.Pages.Users;

using GtPrax.Application.Models;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzerverwaltung", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "admin,manager")]
public sealed class IndexModel : PageModel
{
    private readonly IUserRepository _users;
    public UserDto[] Users { get; private set; } = Array.Empty<UserDto>();
    public int UsersConfirmed { get; set; }
    public int UsersNotConfirmed { get; set; }
    public int UsersLocked { get; set; }

    public IndexModel(IUserRepository users)
    {
        _users = users;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Users = await _users.GetAll(cancellationToken);
        UsersConfirmed = Users.Count(u => u.IsEmailConfirmed);
        UsersNotConfirmed = Users.Count(u => !u.IsEmailConfirmed);
        UsersLocked = Users.Count(u => u.IsLocked);
    }
}
