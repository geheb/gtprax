namespace GtPrax.WebApp.Pages.Users;

using GtPrax.Application.Models;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Benutzer anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "admin,manager")]
public sealed class CreateUserModel : PageModel
{
    private readonly IUserRepository _users;

    [BindProperty]
    public CreateUserInput Input { get; set; } = new CreateUserInput();

    public CreateUserModel(IUserRepository users)
    {
        _users = users;
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

        if (Input.Roles.All(r => r == false))
        {
            ModelState.AddModelError(string.Empty, "Keine Rolle ausgewählt.");
            return Page();
        }

        var user = new UserDto();
        Input.ToDto(user);

        var errors = await _users.Create(user, cancellationToken);
        if (errors != null)
        {
            errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e));
            return Page();
        }

        return RedirectToPage("Index");
    }
}
