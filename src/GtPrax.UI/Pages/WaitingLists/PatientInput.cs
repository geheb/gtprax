namespace GtPrax.UI.Pages.WaitingLists;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GtPrax.Application.UseCases.PatientFiles;
using GtPrax.UI.Attributes;

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

    public CreatePatientFileDto ToDto()
    {
        var tags = Array.Empty<PatientFileTag>();
        if (IsPriorityTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientFileTag.Priority;
        }
        if (IsJumperTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientFileTag.Jumper;
        }
        if (IsNeurofeedbackTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientFileTag.Neurofeedback;
        }

        var days = TherapyTimes.ToDto();
        var birthdate = DateOnly.ParseExact(Birthday!, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        return new(Name!, birthdate, PhoneNumber!, Reason, Doctor, days, tags, Remark);
    }
}
