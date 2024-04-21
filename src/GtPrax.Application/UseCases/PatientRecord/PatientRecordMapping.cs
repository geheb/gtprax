namespace GtPrax.Application.UseCases.PatientRecord;

using GtPrax.Application.Converter;
using GtPrax.Domain.Models;
using GtPrax.Domain.ValueObjects;

internal static class PatientRecordMapping
{
    public static PatientRecordIndexItemDto MapToIndexDto(this PatientRecord item, string? lastModiedBy, GermanDateTimeConverter dateTimeConverter) =>
        new(Id: item.Id,
            LastModified: dateTimeConverter.ToLocal(item.Audit.LastModifiedDate ?? item.Audit.CreatedDate),
            LastModifedBy: lastModiedBy,
            Name: item.Person.Name,
            BirthDate: item.Person.BirthDate,
            PhoneNumber: item.Person.PhoneNumber,
            ReferralReason: item.Referral?.Reason,
            Tags: item.Tags.Select(t => (PatientRecordTag)t.Key).ToArray(),
            HasTherapyDaysWithHomeVisit: item.TherapyDays.Values.Any(t => t.IsHomeVisit));

    public static PatientRecordDto MapToDto(this PatientRecord item) =>
        new(Name: item.Person.Name,
            BirthDate: item.Person.BirthDate,
            PhoneNumber: item.Person.PhoneNumber,
            ReferralReason: item.Referral?.Reason,
            ReferralDoctor: item.Referral?.Doctor,
            TherapyDays: item.TherapyDays.Select(k => k.Value.MapToDto(k.Key)).ToArray(),
            Tags: item.Tags.Select(t => (PatientRecordTag)t.Key).ToArray(),
            Remark: item.Remark);

    public static TherapyDayDto MapToDto(this TherapyDay item, DayOfWeek day) =>
        new(Day: day,
            IsMorning: item.IsMorning,
            IsAfternoon: item.IsAfternoon,
            IsHomeVisit: item.IsHomeVisit,
            AvailableFrom: item.AvailableFrom);

    public static IReadOnlyDictionary<DayOfWeek, TherapyDay> MapToDomain(this IEnumerable<TherapyDayDto> items) =>
        items.ToDictionary(t => t.Day, t => new TherapyDay(t.IsMorning, t.IsAfternoon, t.IsHomeVisit, t.AvailableFrom));

    public static TagType[] MapToDomain(this IEnumerable<PatientRecordTag> items) =>
        items.Select(t => TagType.From((int)t)).ToArray();
}
