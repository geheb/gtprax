namespace GtPrax.UI.Pages.WaitingList;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Patient(in) bearbeiten", FromPage = typeof(PatientsModel))]
[Authorize]
public class EditPatientModel : PageModel
{
    public string? Id { get; set; }

    public void OnGet(string id) => Id = id;
}
