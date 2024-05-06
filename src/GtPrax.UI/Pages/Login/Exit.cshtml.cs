namespace GtPrax.UI.Pages.Login;

using GtPrax.Application.UseCases.Login;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class ExitModel : PageModel
{
    private readonly IMediator _mediator;

    public ExitModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await _mediator.Send(new SignOutCommand(), cancellationToken);
        return LocalRedirect(Url.Content("~/"));
    }
}
