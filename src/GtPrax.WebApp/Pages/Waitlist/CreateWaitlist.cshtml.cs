namespace GtPrax.WebApp.Pages.Waitlist;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Warteliste anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "admin,manager")]
public sealed class CreateWaitlistModel : PageModel
{
    private readonly IWaitlistRepository _waitlists;

    [BindProperty]
    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    public CreateWaitlistModel(IWaitlistRepository waitlists)
    {
        _waitlists = waitlists;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _waitlists.Create(Name!, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, I18n.Messages.InternalErrorCreateRecord);
            return Page();
        }

        return RedirectToPage("/Waitlist/Index");
    }
}
