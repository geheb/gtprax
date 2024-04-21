namespace GtPrax.UI.Pages.WaitingList;

using GtPrax.UI.Attributes;
using System.ComponentModel.DataAnnotations;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GtPrax.Application.UseCases.WaitingList;
using GtPrax.UI.Extensions;
using GtPrax.Application.UseCases.UserAccount;

[Node("Warteliste anlegen", FromPage = typeof(IndexModel))]
[Authorize]
public class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    [BindProperty]
    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    public CreateModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _mediator.Send(new CreateWaitingListCommand(Name!, User.GetId()!), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage(this.PageLinkName<IndexModel>());
    }
}
