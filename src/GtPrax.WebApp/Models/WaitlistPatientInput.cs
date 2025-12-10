namespace GtPrax.WebApp.Models;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GtPrax.Application.Models;
using GtPrax.Infrastructure.AspNetCore;

public sealed class WaitlistPatientInput
{
    public DateTimeOffset? Created { get; set; }
    public Guid? UserId { get; set; }
    public Guid? WaitlistId { get; set; }

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
    public TherapyTimesInput TherapyTimes { get; set; } = new TherapyTimesInput();

    [Display(Name = "Bemerkung")]
    [TextLengthField(1024)]
    public string? Remark { get; set; }

    [Display(Name = "Priorität")]
    public bool IsPriority { get; set; }

    [Display(Name = "Springer")]
    public bool IsJumper { get; set; }

    [Display(Name = "Neurofeedback")]
    public bool IsNeurofeedback { get; set; }

    [Display(Name = "Schule")]
    public bool IsSchool { get; set; }

    [Display(Name = "Kita")]
    public bool IsDaycare { get; set; }

    [Display(Name = "Gruppe")]
    public bool IsGroup { get; set; }

    public WaitlistPatientDto ToDto()
    {
        TagsDto? tags = null;
        if (IsPriority || IsJumper || IsNeurofeedback || IsSchool || IsDaycare || IsGroup)
        {
            tags = new TagsDto
            {
                IsPriority = IsPriority,
                IsJumper = IsJumper,
                IsNeurofeedback = IsNeurofeedback,
                IsSchool = IsSchool,
                IsDaycare = IsDaycare,
                IsGroup = IsGroup
            };
        }

        return new WaitlistPatientDto
        {
            Created = Created,
            UserId = UserId,
            WaitlistId = WaitlistId,
            Name = Name?.Trim(),
            Birthday = DateOnly.TryParse(Birthday, out var b) ? b : null,
            PhoneNumber = PhoneNumber?.Trim(),
            Reason = Reason?.Trim(),
            Doctor = Doctor?.Trim(),
            Remark = Remark?.Trim(),
            TherapyTimes = TherapyTimes.Map(),
            Tags = tags
        };
    }

    public void FromDto(WaitlistPatientDto dto)
    {
        Name = dto.Name;
        Birthday = dto.Birthday?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        PhoneNumber = dto.PhoneNumber;
        Reason = dto.Reason;
        Doctor = dto.Doctor;
        Remark = dto.Remark;
        TherapyTimes.Map(dto.TherapyTimes);
        IsPriority = dto.Tags?.IsPriority ?? false;
        IsJumper = dto.Tags?.IsJumper ?? false;
        IsNeurofeedback = dto.Tags?.IsNeurofeedback ?? false;
        IsSchool = dto.Tags?.IsSchool ?? false;
        IsDaycare = dto.Tags?.IsDaycare ?? false;
        IsGroup = dto.Tags?.IsGroup ?? false;
    }
}
