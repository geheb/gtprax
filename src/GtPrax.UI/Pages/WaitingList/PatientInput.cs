namespace GtPrax.UI.Pages.WaitingList;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Converter;
using GtPrax.Application.UseCases.PatientRecord;
using GtPrax.UI.Attributes;

public class PatientInput
{
    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    [Display(Name = "Geburtsdatum")]
    [RequiredField]
    public string? BirthDate { get; set; }

    [Display(Name = "Telefonnummer")]
    [RequiredField, PhoneField]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Verordnungsgrund")]
    [TextLengthField]
    public string? Reason { get; set; }

    [Display(Name = "Verordnungsarzt")]
    [TextLengthField]
    public string? Doctor { get; set; }

    [Display(Name = "Therapiezeiten")]
    public TherapyDaysInput TherapyDays { get; set; } = new();

    [Display(Name = "Bemerkung")]
    [TextLengthField(1024)]
    public string? Remark { get; set; }

    [Display(Name = "Priorität")]
    public bool IsPriorityTag { get; set; }

    [Display(Name = "Springer")]
    public bool IsJumperTag { get; set; }

    [Display(Name = "Neurofeedback")]
    public bool IsNeurofeedbackTag { get; set; }

    public CreatePatientRecordDto ToCreateDto(GermanDateTimeConverter dateTimeConverter)
    {
        var tags = Array.Empty<PatientRecordTag>();
        if (IsPriorityTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientRecordTag.Priority;
        }
        if (IsJumperTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientRecordTag.Jumper;
        }
        if (IsNeurofeedbackTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientRecordTag.Neurofeedback;
        }

        var days = TherapyDays.ToDto(dateTimeConverter);
        var birthdate = dateTimeConverter.FromIsoDate(BirthDate);

        return new(Name!.Trim(), birthdate!.Value, PhoneNumber!.Trim(), Reason?.Trim(), Doctor?.Trim(), days, tags, Remark?.Trim());
    }

    public UpdatePatientRecordDto ToUpdateDto(GermanDateTimeConverter dateTimeConverter)
    {
        var tags = Array.Empty<PatientRecordTag>();
        if (IsPriorityTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientRecordTag.Priority;
        }
        if (IsJumperTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientRecordTag.Jumper;
        }
        if (IsNeurofeedbackTag)
        {
            Array.Resize(ref tags, tags.Length + 1);
            tags[^1] = PatientRecordTag.Neurofeedback;
        }

        var days = TherapyDays.ToDto(dateTimeConverter);
        var birthdate = dateTimeConverter.FromIsoDate(BirthDate);

        return new(PhoneNumber!.Trim(), Reason?.Trim(), Doctor?.Trim(), days, tags, Remark?.Trim());
    }

    public void FromDto(PatientRecordItemDto dto, GermanDateTimeConverter dateTimeConverter)
    {
        Name = dto.Name;
        BirthDate = dateTimeConverter.ToIso(dto.BirthDate);
        PhoneNumber = dto.PhoneNumber;
        Reason = dto.ReferralReason;
        Doctor = dto.ReferralDoctor;
        TherapyDays.FromDto(dto.TherapyDays, dateTimeConverter);
        Remark = dto.Remark;
        IsPriorityTag = dto.Tags.Contains(PatientRecordTag.Priority);
        IsJumperTag = dto.Tags.Contains(PatientRecordTag.Jumper);
        IsNeurofeedbackTag = dto.Tags.Contains(PatientRecordTag.Neurofeedback);
    }
}
