namespace GtPrax.WebApp.Pages.MyAccount;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Mein Konto", FromPage = typeof(Pages.IndexModel))]
[Authorize]
public sealed class IndexModel : PageModel
{
    private readonly IUserRepository _users;

    [BindProperty, Display(Name = "E-Mail-Adresse")]
    public string? Email { get; set; }

    [BindProperty, Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    public bool IsDisabled { get; set; }
    public string? Info { get; set; }

    public IndexModel(IUserRepository users)
    {
        _users = users;
    }

    public async Task OnGetAsync(int? message, CancellationToken cancellationToken)
    {
        Info = message switch
        {
            0 => "Änderungen wurden gespeichert.",
            1 => "Das Passwort wurde geändert.",
            2 => "Eine E-Mail wird an die neue E-Mail-Adresse versendet und muss bestätigt werden - erst dann ist die Änderung vollständig.",
            _ => default
        };

        var user = await _users.Find(User.GetId(), cancellationToken);
        if (user == null)
        {
            IsDisabled = true;
            return;
        }

        Name = user.Name;
        Email = user.Email;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var errors = await _users.Update(User.GetId(), Name!);
        if (errors != null)
        {
            errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e));
            return Page();
        }

        return RedirectToPage(new { message = 0 });
    }
}
