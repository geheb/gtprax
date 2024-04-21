namespace GtPrax.UI.Pages.WaitingList;

using GtPrax.Application.UseCases.PatientRecord;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Patient(inn)en", FromPage = typeof(IndexModel))]
[Authorize]
public class PatientsModel : PageModel
{
    private readonly IMediator _mediator;

    public string? Id { get; set; }
    public string? WaitingListName { get; set; }
    public PatientRecordIndexItemDto[] Items { get; set; } = [];

    public PatientsModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(string id, CancellationToken cancellationToken)
    {
        Id = id;

        var result = await _mediator.Send(new GetPatientRecordIndexQuery(id), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return;
        }

        WaitingListName = result.Value.WaitingListName;

        Items = result.Value.Items;
    }
}
