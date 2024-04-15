namespace GtPrax.Application.UseCases.PatientFiles;

using GtPrax.Domain.Entities;

internal static class PatientFileMapping
{
    public static PatientFileDto MapToDto(this PatientFile item) =>
        new(Name: item.Person.Identity.Name,
            BirthDate: item.Person.Identity.BirthDate,
            PhoneNumber: item.Person.PhoneNumber,
            ReferralReason: item.Referral?.Reason,
            ReferralDoctor: item.Referral?.Doctor,
            TherapyDays: item.TherapyDays.Select(k => k.Value.MapToDto(k.Key)).ToArray(),
            Tags: item.Tags.Select(t => (PatientFileTag)t.Key).ToArray(),
            Remark: item.Remark);

    public static TherapyDayDto MapToDto(this TherapyDay item, DayOfWeek day) =>
        new(Day: day,
            IsMorning: item.IsMorning,
            IsAfternoon: item.IsAfternoon,
            IsHomeVisit: item.IsHomeVisit,
            AvailableFrom: item.AvailableFrom);
}
