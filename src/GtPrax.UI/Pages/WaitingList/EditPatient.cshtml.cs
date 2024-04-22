namespace GtPrax.UI.Pages.WaitingList;

using System.Threading;
using GtPrax.Application.Converter;
using GtPrax.Application.UseCases.PatientRecord;
using GtPrax.Application.UseCases.UserAccount;
using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Patient(in) bearbeiten", FromPage = typeof(PatientsModel))]
[Authorize]
public class EditPatientModel : PageModel
{
    private readonly IMediator _mediator;

    public string? Id { get; set; }
    public string? WaitingListId { get; set; }
    public string? WaitingListName { get; set; }
    public bool IsDisabled { get; set; }
    public string? Created { get; set; }
    public string? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    [BindProperty]
    public PatientInput Input { get; set; } = new();

    public EditPatientModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(string id, CancellationToken cancellationToken)
    {
        var patient = await UpdateView(id, cancellationToken);
        if (patient is null)
        {
            return;
        }
        Input.FromDto(patient, new());
    }

    public async Task<IActionResult> OnPostAsync(string id, CancellationToken cancellationToken)
    {
        if (await UpdateView(id, cancellationToken) is null)
        {
            return Page();
        }

        var dto = Input.ToUpdateDto(new());
        var result = await _mediator.Send(new UpdatePatientRecordCommand(id, User.GetId()!, dto), cancellationToken);

        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage(this.PageLinkName<PatientsModel>(), new { id = WaitingListId });
    }

    private async Task<PatientRecordItemDto?> UpdateView(string id, CancellationToken cancellationToken)
    {
        Id = id;

        var dto = await _mediator.Send(new FindPatientRecordByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            ModelState.AddModelError(string.Empty, "Der/Die Patient(in) wurde nicht gefunden.");
            IsDisabled = true;
            return null;
        }

        GermanDateTimeConverter dateTimeConverter = new();

        WaitingListId = dto.WaitingListId;
        WaitingListName = dto.WaitingListName;
        Created = dateTimeConverter.ToDateTime(dto.Patient.Created);
        LastModified = dto.Patient.LastModified is not null ? dateTimeConverter.ToDateTime(dto.Patient.LastModified!.Value) : null;
        LastModifiedBy = dto.Patient.LastModifiedBy;

        return ModelState.IsValid ? dto.Patient : null;
    }
}
