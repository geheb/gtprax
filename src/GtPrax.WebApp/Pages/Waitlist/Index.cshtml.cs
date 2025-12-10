namespace GtPrax.WebApp.Pages.Waitlist;

using GtPrax.Application.Models;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Wartelisten", FromPage = typeof(Pages.IndexModel))]
[Authorize]
public sealed class IndexModel : PageModel
{
    private readonly IWaitlistRepository _waitlists;

    public WaitlistDto[] Items { get; set; } = Array.Empty<WaitlistDto>();

    public IndexModel(IWaitlistRepository waitlists)
    {
        _waitlists = waitlists;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken) =>
        Items = await _waitlists.GetAll(cancellationToken);
}
