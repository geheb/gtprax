namespace GtPrax.Infrastructure.Stores;

using System.Globalization;
using GtPrax.Domain.Entities;
using GtPrax.Domain.ValueObjects;
using MongoDB.Bson;

internal static class PatientFileMapping
{
    public static PatientFileModel MapToModel(this PatientFile entity) => new()
    {
        Id = string.IsNullOrEmpty(entity.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(entity.Id),
        WaitingListId = ObjectId.Parse(entity.WaitingListId),
        CreatedDate = entity.Audit.CreatedDate,
        CreatedById = ObjectId.Parse(entity.Audit.CreatedBy),
        LastModifiedDate = entity.Audit.LastModifiedDate,
        LastModifiedById = entity.Audit.LastModifiedBy is not null ? ObjectId.Parse(entity.Audit.LastModifiedBy) : null,
        Name = entity.Person.Identity.Name,
        BirthDate = new DateTime(entity.Person.Identity.BirthDate, TimeOnly.MinValue, DateTimeKind.Unspecified),
        PhoneNumber = entity.Person.PhoneNumber,
        ReferralReason = entity.Referral?.Reason,
        ReferralDoctor = entity.Referral?.Doctor,
        TherapyDays = entity.TherapyDays.Select(t => t.Value.MapToModel(t.Key)).ToArray(),
        Tags = entity.Tags.Select(t => t.Key).ToArray(),
        Remark = entity.Remark
    };

    public static TherapyDayModel MapToModel(this TherapyDay entity, DayOfWeek day) => new()
    {
        Day = day,
        IsMorning = entity.IsMorning,
        IsAfternoon = entity.IsAfternoon,
        IsHomeVisit = entity.IsHomeVisit,
        AvailableFrom = entity.AvailableFrom?.ToString(CultureInfo.InvariantCulture)
    };

    public static PatientFile MapToDomain(this PatientFileModel model, DateTimeOffset now)
    {
        var personResult = new PersonBuilder()
            .SetName(model.Name)
            .SetBirthDate(DateOnly.FromDateTime(model.BirthDate))
            .SetPhoneNumber(model.PhoneNumber)
            .Build([], now);

        if (personResult.IsFailed)
        {
            throw new InvalidOperationException($"Inconsistency of {nameof(Person)} detected: {model.Id}");
        }

        var patientFileBuilder = new PatientFileBuilder()
            .SetId(model.Id.ToString())
            .SetWaitingListId(model.WaitingListId.ToString())
            .SetPerson(personResult.Value)
            .SetReferral(model.ReferralReason, model.ReferralDoctor)
            .SetRemark(model.Remark)
            .SetCreated(model.CreatedById.ToString(), model.CreatedDate)
            .SetLastModified(model.LastModifiedById?.ToString(), model.LastModifiedDate);

        Array.ForEach(model.TherapyDays ?? [], t => patientFileBuilder.SetTherapyDay(t.Day, t.IsMorning, t.IsAfternoon, t.IsHomeVisit, TimeOnly.TryParse(t.AvailableFrom, out var time) ? time : null));
        Array.ForEach(model.Tags ?? [], t => patientFileBuilder.SetTag(TagType.From(t)));

        var patientFileResult = patientFileBuilder.Build([]);

        if (patientFileResult.IsFailed)
        {
            throw new InvalidOperationException($"Inconsistency of {nameof(PatientFile)} detected: {model.Id}");
        }

        return patientFileResult.Value;
    }

    public static PatientFile[] MapToDomain(this IEnumerable<PatientFileModel> models, DateTimeOffset now) =>
        models.Select(m => m.MapToDomain(now)).ToArray();

    public static PersonIdentity MapToIdentityDomain(this PatientFileModel model) =>
        new(model.Name, DateOnly.FromDateTime(model.BirthDate));

    public static PersonIdentity[] MapToIdentityDomain(this IEnumerable<PatientFileModel> models) =>
        models.Select(m => m.MapToIdentityDomain()).ToArray();
}
