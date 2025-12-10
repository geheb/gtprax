namespace GtPrax.WebApp.Pages.Waitlist;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Converter;
using GtPrax.Application.Models;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Node("Patienten", FromPage = typeof(IndexModel))]
[Authorize]
public sealed class PatientsModel : PageModel
{
    private readonly IWaitlistRepository _waitlists;

    public Guid WaitlistId { get; set; }
    public string? WaitlistName { get; set; } = "n.v.";

    [Display(Name = "Suche")]
    [BindProperty, TextLengthField(256, MinimumLength = 3)]
    public string? SearchKey { get; set; }

    public WaitlistPatientDto[] Items { get; private set; } = [];
    public SelectListItem[] SelectItems { get; private set; } = [];
    public int Total { get; set; }

    public PatientsModel(IWaitlistRepository waitlists)
    {
        _waitlists = waitlists;
    }

    public async Task OnGetAsync(Guid waitlistId, int filter, CancellationToken cancellationToken = default)
    {
        if (!await UpdateView(waitlistId, filter, cancellationToken))
        {
            return;
        }

        Items = await FilterItems(waitlistId, filter, cancellationToken);
    }

    public async Task OnPostAsync(Guid waitlistId, int filter, CancellationToken cancellationToken)
    {
        if (!await UpdateView(waitlistId, filter, cancellationToken))
        {
            return;
        }

        Items = await FilterItems(waitlistId, filter, cancellationToken);
    }

    private async Task<WaitlistPatientDto[]> FilterItems(Guid waitlistId, int filter, CancellationToken cancellationToken)
    {
        var items = await _waitlists.GetPatients(waitlistId, SearchKey, cancellationToken);
        Total = items.Length;
        if (filter is < 1 or > 11)
        {
            return items;
        }

        var today = new GermanDateTimeConverter().ToLocal(DateTimeOffset.UtcNow).Date;

        return filter switch
        {
            1 => items.Where(i => i.CalcAge(today) <= 17 && i.HasTherapyTimesMorning).ToArray(),
            2 => items.Where(i => i.CalcAge(today) <= 17 && i.HasTherapyTimesAfternoon).ToArray(),
            3 => items.Where(i => i.CalcAge(today) >= 18 && i.HasTherapyTimesMorning).ToArray(),
            4 => items.Where(i => i.CalcAge(today) >= 18 && i.HasTherapyTimesAfternoon).ToArray(),
            5 => items.Where(i => i.HasTherapyTimesHomeVisit).ToArray(),
            6 => items.Where(i => i.Tags?.IsPriority == true).ToArray(),
            7 => items.Where(i => i.Tags?.IsJumper == true).ToArray(),
            8 => items.Where(i => i.Tags?.IsNeurofeedback == true).ToArray(),
            9 => items.Where(i => i.Tags?.IsSchool == true).ToArray(),
            10 => items.Where(i => i.Tags?.IsDaycare == true).ToArray(),
            11 => items.Where(i => i.Tags?.IsGroup == true).ToArray(),
            _ => throw new ArgumentOutOfRangeException(nameof(filter))
        };
    }

    private async Task<bool> UpdateView(Guid waitlistId, int filter, CancellationToken cancellationToken)
    {
        WaitlistId = waitlistId;
        var waitlist = await _waitlists.Find(waitlistId, cancellationToken);
        if (waitlist == null)
        {
            ModelState.AddModelError(string.Empty, I18n.Messages.WaitlistNotFound);
            return false;
        }
        WaitlistName = waitlist.Name;

        SelectItems =
        [
            new SelectListItem("Alle", "0", filter is < 1 or > 11),
            new SelectListItem("Kinder vormittags", "1", filter == 1),
            new SelectListItem("Kinder nachmittags", "2", filter == 2),
            new SelectListItem("Erwachsene vormittags", "3", filter == 3),
            new SelectListItem("Erwachsene nachmittags", "4", filter == 4),
            new SelectListItem("Hausbesuche", "5", filter == 5),
            new SelectListItem("Priorität", "6", filter == 6),
            new SelectListItem("Springer", "7", filter == 7),
            new SelectListItem("Neurofeedback", "8", filter == 8),
            new SelectListItem("Schule", "9", filter == 9),
            new SelectListItem("Kita", "10", filter == 10),
            new SelectListItem("Gruppe", "11", filter == 11),
        ];

        return ModelState.IsValid;
    }
}
