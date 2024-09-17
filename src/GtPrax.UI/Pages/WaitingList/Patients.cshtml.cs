namespace GtPrax.UI.Pages.WaitingList;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
using GtPrax.Application.UseCases.PatientRecord;
using GtPrax.Application.UseCases.WaitingList;
using GtPrax.UI.Attributes;
using GtPrax.UI.Models;
using GtPrax.UI.Routing;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Node("Patient(inn)en", FromPage = typeof(IndexModel))]
[Authorize(Policy = Policies.Require2fa)]
public class PatientsModel : PageModel
{
    private readonly IMediator _mediator;

    public string? Id { get; set; }
    public string? WaitingListName { get; set; }
    public PatientRecordIndexItemDto[] Items { get; set; } = [];
    public SelectListItem[] FilterItems { get; set; } = [];
    public bool IsDisabled { get; set; }
    public int TotalCount { get; set; }

    [BindProperty, TextLengthField(100), Display(Name = "Suchbegriffe")]
    public string? SearchTerms { get; set; }

    public PatientsModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task OnGetAsync(string id, int filter, CancellationToken cancellationToken) =>
        UpdateView(id, filter, cancellationToken);

    public Task OnPostAsync(string id, int filter, CancellationToken cancellationToken) =>
        UpdateView(id, filter, cancellationToken);

    private async Task<bool> UpdateView(string id, int filter, CancellationToken cancellationToken)
    {
        Id = id;

        var waitingList = await _mediator.Send(new FindWaitingListByIdQuery(id), cancellationToken);
        if (waitingList is null)
        {
            ModelState.AddModelError(string.Empty, "Die Warteliste wurde nicht gefunden.");
            IsDisabled = true;
            return false;
        }

        var filterType = Enum.IsDefined((PatientRecordFilter)filter) ? (PatientRecordFilter)filter : PatientRecordFilter.None;
        FilterItems =
        [
            new("Alle",
                ((int)PatientRecordFilter.None).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.None),

            new("Kinder vormittags",
                ((int)PatientRecordFilter.ChildrenInTheMorning).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.ChildrenInTheMorning),

            new("Kinder nachmittags",
                ((int)PatientRecordFilter.ChildrenInTheAfternoon).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.ChildrenInTheAfternoon),

            new("Erwachsene vormittags",
                ((int)PatientRecordFilter.AdultsInTheMorning).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.AdultsInTheMorning),

            new("Erwachsene nachmittags",
                ((int)PatientRecordFilter.AdultsInTheAfternoon).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.AdultsInTheAfternoon),

            new("Hausbesuche",
                ((int)PatientRecordFilter.TherapyDayWithHomeVisit).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.TherapyDayWithHomeVisit),

            new("Priorität",
                ((int)PatientRecordFilter.HasTagPriority).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.HasTagPriority),

            new("Springer",
                ((int)PatientRecordFilter.HasTagJumper).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.HasTagJumper),

            new("Neurofeedback",
                ((int)PatientRecordFilter.HasTagNeurofeedback).ToString(CultureInfo.InvariantCulture),
                filterType is PatientRecordFilter.HasTagNeurofeedback)
        ];

        var result = await _mediator.Send(new GetPatientsBySearchQuery(id, SearchTerms, filterType), cancellationToken);

        WaitingListName = waitingList.Name;
        Items = result.Items;
        TotalCount = result.TotalCount;
        return true;
    }
}
