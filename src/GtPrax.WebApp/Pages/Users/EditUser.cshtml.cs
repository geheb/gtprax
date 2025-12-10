namespace GtPrax.WebApp.Pages.Users;

using GtPrax.WebApp.Models;
using GtPrax.Infrastructure.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GtPrax.Application.Repositories;

[Node("Benutzer bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "admin,manager")]
public sealed class EditUserModel : PageModel
{
    private readonly IUserRepository _users;

    public Guid? Id { get; set; }
    public string? Name { get; set; }

    [BindProperty]
    public EditUserInput Input { get; set; } = new EditUserInput();

    public bool IsDisabled { get; set; }

    public EditUserModel(IUserRepository users)
    {
        _users = users;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Id = id;
        if (!ModelState.IsValid)
        {
            return;
        }

        var user = await _users.Find(id, cancellationToken);
        if (user == null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Benutzer wurde nicht gefunden.");
            return;
        }

        Name = user.Name ?? user.Email;

        Input.FromDto(user);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        Id = id;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _users.Find(id, cancellationToken);
        if (user == null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Benutzer wurde nicht gefunden.");
            return Page();
        }

        if (Input.Roles.All(r => r == false))
        {
            ModelState.AddModelError(string.Empty, "Keine Rolle ausgewählt.");
            return Page();
        }

        Name = user.Name ?? user.Email;

        Input.ToDto(user);

        var errors = await _users.Update(user, Input.Password, cancellationToken);
        if (errors != null)
        {
            errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e));
            return Page();
        }

        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostConfirmEmailAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _users.NotifyConfirmRegistration(id, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostRemoveUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _users.Remove(id, cancellationToken);
        return new JsonResult(result);
    }
}
