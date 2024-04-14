namespace GtPrax.UI.Pages.WaitingLists;

using GtPrax.Application.UseCases.UserAccounts;
using GtPrax.Application.UseCases.PatientFiles;
using GtPrax.UI.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GtPrax.UI.Extensions;

[Node("Patient(in) anlegen", FromPage = typeof(PatientsModel))]
[Authorize]
public class CreatePatientModel : PageModel
{
    private readonly IMediator _mediator;

    public string? Id { get; set; }
    public string? Details { get; set; }

    [BindProperty]
    public PatientInput Input { get; set; } = new();

    public CreatePatientModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void OnGet(string id) => Id = id;

    public async Task<IActionResult> OnPostAsync(string id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _mediator.Send(new CreatePatientFileCommand(id, User.GetId()!, Input.ToDto()), cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage(this.PageLinkName<PatientsModel>(), new { id });
    }
}
