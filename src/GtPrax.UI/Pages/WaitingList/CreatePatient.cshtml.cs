namespace GtPrax.UI.Pages.WaitingList;

using GtPrax.Application.UseCases.PatientRecord;
using GtPrax.Application.UseCases.UserAccount;
using GtPrax.Application.UseCases.WaitingList;
using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Patient(in) anlegen", FromPage = typeof(PatientsModel))]
[Authorize]
public class CreatePatientModel : PageModel
{
    private readonly IMediator _mediator;

    public string? Id { get; set; }
    public string? WaitingListName { get; set; }
    public bool IsDisabled { get; set; }

    [BindProperty]
    public PatientInput Input { get; set; } = new();

    public CreatePatientModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(string id, CancellationToken cancellationToken) =>
        await UpdateView(id, cancellationToken);

    public async Task<IActionResult> OnPostAsync(string id, CancellationToken cancellationToken)
    {
        if (!await UpdateView(id, cancellationToken))
        {
            return Page();
        }

        var dto = Input.ToCreateDto(new());
        var result = await _mediator.Send(new CreatePatientRecordCommand(id, User.GetId()!, dto), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage(this.PageLinkName<PatientsModel>(), new { id });
    }

    private async Task<bool> UpdateView(string id, CancellationToken cancellationToken)
    {
        Id = id;
        var waitingList = await _mediator.Send(new FindWaitingListByIdQuery(id), cancellationToken);
        if (waitingList is null)
        {
            ModelState.AddModelError(string.Empty, "Die Warteliste wurde nicht gefunden.");
            IsDisabled = true;
            return false;
        }
        WaitingListName = waitingList.Name;
        return ModelState.IsValid;
    }
}
