namespace GtPrax.WebApp.Pages.Waitlist;

using GtPrax.Application.Converter;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.Infrastructure.Extensions;
using GtPrax.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Node("Patient bearbeiten", FromPage = typeof(PatientsModel))]
[Authorize]
public sealed class EditPatientModel : PageModel
{
    private readonly IWaitlistRepository _waitlists;
    private readonly GermanDateTimeConverter _dateTimeConverter = new();

    public SelectListItem[] SelectWaitlistItems { get; private set; } = [];
    public Guid? WaitlistId { get; set; }
    public Guid? Id { get; set; }
    public string? WaitlistName { get; set; }

    [BindProperty]
    public WaitlistPatientInput Input { get; set; } = new WaitlistPatientInput();

    public string? CreatedAt { get; set; }
    public string? LastUpdatedAt { get; set; }
    public string? LastUpdatedBy { get; set; }
    public bool IsDisabled { get; set; }

    public EditPatientModel(IWaitlistRepository waitlists)
    {
        _waitlists = waitlists;
    }

    public async Task OnGetAsync(Guid waitlistId, Guid id, CancellationToken cancellationToken)
    {
        var waitLists = await _waitlists.GetAll(cancellationToken);
        SelectWaitlistItems = waitLists.Where(w => w.Id != waitlistId).Select(w => new SelectListItem(w.Name, w.Id.ToString())).ToArray();
        if (SelectWaitlistItems.Length > 0)
        {
            SelectWaitlistItems[0].Selected = true;
        }

        await UpdateView(waitlistId, id, true, cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(Guid waitlistId, Guid id, CancellationToken cancellationToken)
    {
        if (!await UpdateView(waitlistId, id, false, cancellationToken))
        {
            return Page();
        }

        var dto = Input.ToDto();
        dto.Id = id;
        dto.WaitlistId = waitlistId;
        dto.UserId = User.GetId();

        var result = await _waitlists.Update(dto, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, I18n.Messages.InternalErrorUpdateRecord);
            return Page();
        }

        return RedirectToPage("/Waitlist/Patients", new { waitlistId });
    }

    public async Task<IActionResult> OnPostDeletePatientAsync(Guid waitlistId, Guid id, CancellationToken cancellationToken)
    {
        var result = await _waitlists.DeletePatient(id, waitlistId, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostMovePatientAsync(Guid targetWaitlistId, Guid id, CancellationToken cancellationToken)
    {
        var result = await _waitlists.MovePatient(id, targetWaitlistId, cancellationToken);
        return new JsonResult(result);
    }

    private async Task<bool> UpdateView(Guid waitlistId, Guid id, bool updatePatient, CancellationToken cancellationToken)
    {
        WaitlistId = waitlistId;
        Id = id;
        var waitlist = await _waitlists.Find(waitlistId, cancellationToken);
        if (waitlist == null)
        {
            ModelState.AddModelError(string.Empty, I18n.Messages.WaitlistNotFound);
            IsDisabled = true;
            return false;
        }
        WaitlistName = waitlist.Name;

        var patient = await _waitlists.FindPatient(id, cancellationToken);
        if (patient == null || patient.WaitlistId != waitlistId)
        {
            ModelState.AddModelError(string.Empty, I18n.Messages.WaitlistPatientNotFound);
            IsDisabled = true;
            return false;
        }

        if (updatePatient)
        {
            Input.FromDto(patient);
        }

        CreatedAt = patient.Created.HasValue ? _dateTimeConverter.ToDateTime(patient.Created.Value) : null;
        LastUpdatedAt = patient.Updated.HasValue ? _dateTimeConverter.ToDateTime(patient.Updated.Value) : null;
        LastUpdatedBy = patient.User;

        return ModelState.IsValid;
    }
}
