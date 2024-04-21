namespace GtPrax.UI.Pages.WaitingList;

using GtPrax.Application.UseCases.WaitingList;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Wartelisten", FromPage = typeof(Pages.IndexModel))]
[Authorize]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public WaitingListIndexDto[] Items { get; set; } = [];

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken) =>
        Items = await _mediator.Send(new GetWaitingListIndexQuery(), cancellationToken);
}
