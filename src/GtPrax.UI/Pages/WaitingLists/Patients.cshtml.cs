namespace GtPrax.UI.Pages.WaitingLists;

using GtPrax.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Patienten", FromPage = typeof(IndexModel))]
[Authorize]
public class PatientsModel : PageModel
{
    public string? Id { get; set; }
    public string? Details { get; set; }

    public void OnGet(string id) => Id = id;
}