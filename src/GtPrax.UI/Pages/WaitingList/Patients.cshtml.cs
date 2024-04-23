namespace GtPrax.UI.Pages.WaitingList;

using System.ComponentModel.DataAnnotations;
using System.Threading;
using GtPrax.Application.UseCases.PatientRecord;
using GtPrax.UI.Attributes;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Patient(inn)en", FromPage = typeof(IndexModel))]
[Authorize]
public class PatientsModel : PageModel
{
    private readonly IMediator _mediator;

    public string? Id { get; set; }
    public string? WaitingListName { get; set; }
    public PatientRecordIndexItemDto[] Items { get; set; } = [];

    [BindProperty, TextLengthField(100), Display(Name = "Suchbegriffe")]
    public string? SearchTerms { get; set; }

    public PatientsModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task OnGetAsync(string id, CancellationToken cancellationToken) =>
        UpdateView(id, cancellationToken);

    public Task OnPostAsync(string id, CancellationToken cancellationToken) =>
        UpdateView(id, cancellationToken);

    private async Task<bool> UpdateView(string id, CancellationToken cancellationToken)
    {
        Id = id;

        var result = await _mediator.Send(new GetPatientsBySearchTermsQuery(id, SearchTerms), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return false;
        }

        WaitingListName = result.Value.WaitingListName;
        Items = result.Value.Items;
        return true;
    }
}
