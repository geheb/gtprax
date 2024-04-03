namespace GtPrax.UI.Pages.WaitingLists;

using GtPrax.UI.Attributes;
using System.ComponentModel.DataAnnotations;

public class PatientInput
{
    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    [RequiredField, Display(Name = "Geburtsdatum")]
    public string? Birthday { get; set; }

    [Display(Name = "Telefonnummer")]
    [RequiredField, PhoneField]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Verordnungsgrund")]
    [TextLengthField]
    public string? Reason { get; set; }

    [Display(Name = "Arzt")]
    [TextLengthField]
    public string? Doctor { get; set; }

    [Display(Name = "Therapiezeiten")]
    public TherapyTimesInput TherapyTimes { get; set; } = new();

    [Display(Name = "Bemerkung")]
    [TextLengthField(1024)]
    public string? Remark { get; set; }

    [Display(Name = "Priorität")]
    public bool IsPriorityTag { get; set; }

    [Display(Name = "Springer")]
    public bool IsJumperTag { get; set; }

    [Display(Name = "Neurofeedback")]
    public bool IsNeurofeedbackTag { get; set; }
}
