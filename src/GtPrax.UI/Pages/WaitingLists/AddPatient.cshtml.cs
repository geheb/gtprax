namespace GtPrax.UI.Pages.WaitingLists;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Patient hinzufügen", FromPage = typeof(PatientsModel))]
[Authorize]
public class AddPatientModel : PageModel
{
    public string? Id { get; set; }
    public string? Details { get; set; }

    [BindProperty]
    public PatientInput Input { get; set; } = new();

    public void OnGet(string id) => Id = id;
}
