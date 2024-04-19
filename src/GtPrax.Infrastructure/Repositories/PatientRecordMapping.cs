namespace GtPrax.Infrastructure.Repositories;

using System.Globalization;
using GtPrax.Domain.Models;
using GtPrax.Domain.ValueObjects;
using MongoDB.Bson;

internal static class PatientRecordMapping
{
    public static PatientRecordModel MapToModel(this PatientRecord entity) =>
        new()
        {
            Id = ObjectId.Parse(entity.Id.Value),
            WaitingListId = ObjectId.Parse(entity.WaitingListItemId.Value),
            CreatedDate = entity.Audit.CreatedDate,
            CreatedById = ObjectId.Parse(entity.Audit.CreatedBy),
            LastModifiedDate = entity.Audit.LastModifiedDate,
            LastModifiedById = entity.Audit.LastModifiedBy is not null ? ObjectId.Parse(entity.Audit.LastModifiedBy) : null,
            Name = entity.Person.Name,
            BirthDate = new DateTime(entity.Person.BirthDate, TimeOnly.MinValue, DateTimeKind.Unspecified),
            PhoneNumber = entity.Person.PhoneNumber,
            ReferralReason = entity.Referral?.Reason,
            ReferralDoctor = entity.Referral?.Doctor,
            TherapyDays = entity.TherapyDays.Select(t => t.Value.MapToModel(t.Key)).ToArray(),
            Tags = entity.Tags.Select(t => t.Key).ToArray(),
            Remark = entity.Remark
        };

    public static TherapyDayModel MapToModel(this TherapyDay entity, DayOfWeek day) =>
        new()
        {
            Day = day,
            IsMorning = entity.IsMorning,
            IsAfternoon = entity.IsAfternoon,
            IsHomeVisit = entity.IsHomeVisit,
            AvailableFrom = entity.AvailableFrom?.ToString(CultureInfo.InvariantCulture)
        };

    public static PatientRecord MapToDomain(this PatientRecordModel model) =>
        new(new(model.Id.ToString()),
            new(model.WaitingListId.ToString()),
            new(model.CreatedById.ToString(), model.CreatedDate, model.LastModifiedById?.ToString(), model.LastModifiedDate),
            new(model.Name, DateOnly.FromDateTime(model.BirthDate), model.PhoneNumber),
            new(model.ReferralReason, model.ReferralDoctor),
            model.TherapyDays?.ToDictionary(t => t.Day, t => new TherapyDay(t.IsMorning, t.IsAfternoon, t.IsHomeVisit, TimeOnly.TryParse(t.AvailableFrom, out var time) ? time : null)) ?? [],
            model.Tags?.Select(TagType.From).ToArray() ?? [],
            model.Remark);

    public static PatientRecord[] MapToDomain(this IEnumerable<PatientRecordModel> models) =>
        models.Select(m => m.MapToDomain()).ToArray();
}
