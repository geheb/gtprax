namespace GtPrax.WebApp.Pages.Waitlist;

using System.Globalization;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.Infrastructure.Extensions;
using GtPrax.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Patient anlegen", FromPage = typeof(PatientsModel))]
[Authorize]
public sealed class CreatePatientModel : PageModel
{
    private readonly IWaitlistRepository _waitlists;

    public Guid WaitlistId { get; set; }
    public string? WaitlistName { get; set; } = "n.v.";

    [BindProperty]
    public WaitlistPatientInput Input { get; set; } = new WaitlistPatientInput();

    public CreatePatientModel(IWaitlistRepository waitlists)
    {
        _waitlists = waitlists;
    }

    public Task OnGetAsync(Guid waitlistId, CancellationToken cancellationToken) =>
        UpdateView(waitlistId, cancellationToken);

    public async Task<IActionResult> OnPostAsync(Guid waitlistId, CancellationToken cancellationToken)
    {
        if (!await UpdateView(waitlistId, cancellationToken))
        {
            return Page();
        }

        var dto = Input.ToDto();
        dto.WaitlistId = waitlistId;
        dto.UserId = User.GetId();

        var query = dto.Name + " " + dto.Birthday?.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        var otherPatients = await _waitlists.GetPatients(null, query, cancellationToken);
        if (otherPatients.Length > 0)
        {
            var other = otherPatients[0];
            var link = Url.PageLink("/Waitlist/EditPatient", null, new { waitlistId = other.WaitlistId, id = other.Id });

            ModelState.AddModelError(string.Empty, "Patient existiert bereits, siehe " +
                $"<a href=\"{link}\" target=\"_blank\">{other.Name}</a>");

            return Page();
        }

        var result = await _waitlists.CreatePatient(dto, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, I18n.Messages.InternalErrorCreateRecord);
            return Page();
        }

        return RedirectToPage("/Waitlist/Patients", new { waitlistId });
    }

    private async Task<bool> UpdateView(Guid waitlistId, CancellationToken cancellationToken)
    {
        WaitlistId = waitlistId;
        var waitlist = await _waitlists.Find(waitlistId, cancellationToken);
        if (waitlist == null)
        {
            ModelState.AddModelError(string.Empty, I18n.Messages.WaitlistNotFound);
            return false;
        }
        WaitlistName = waitlist.Name;

        return ModelState.IsValid;
    }
}
