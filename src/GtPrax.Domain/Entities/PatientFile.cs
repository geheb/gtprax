namespace GtPrax.Domain.Entities;

using System;
using FluentResults;
using GtPrax.Domain.ValueObjects;

public sealed class PatientFile
{
    private readonly Dictionary<DayOfWeek, TherapyDay> _therapyDays = [];
    private readonly HashSet<TagType> _tags = [];

    public string Id { get; private set; }
    public string WaitingListId { get; private set; }
    public AuditMetadata Audit { get; private set; }
    public Person Person { get; private set; }
    public Referral? Referral { get; private set; }
    public IReadOnlyDictionary<DayOfWeek, TherapyDay> TherapyDays => _therapyDays;
    public IReadOnlyCollection<TagType> Tags => _tags;
    public string? Remark { get; private set; }

    public PatientFile(string waitingListId, AuditMetadata audit, Person person)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(waitingListId);
        ArgumentNullException.ThrowIfNull(audit);
        ArgumentNullException.ThrowIfNull(person);

        Id = string.Empty;
        WaitingListId = waitingListId;
        Audit = audit;
        Person = person;
    }

    public PatientFile(string id, string waitingListId, AuditMetadata audit, Person person)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(audit);
        ArgumentNullException.ThrowIfNull(person);

        Id = id;
        WaitingListId = waitingListId;
        Audit = audit;
        Person = person;
    }

    public void SetReferral(string? reason, string? doctor) =>
        Referral = new(reason, doctor);

    public void ResetTherapyDays() => _therapyDays.Clear();

    public void SetTherapyDay(DayOfWeek day, bool isMorning, bool isAfternoon, bool isHomeVisit, TimeOnly? availableFrom) =>
        _therapyDays[day] = new TherapyDay(isMorning, isAfternoon, isHomeVisit, availableFrom);

    public void ResetTags() => _tags.Clear();

    public void SetTag(TagType tag) => _tags.Add(tag);

    public void SetRemark(string? remark) => Remark = remark;

    public static Result<PatientFile> Create(string waitingListId, DateTimeOffset createdDate, string createdBy, Person person, WaitingListIdentity[] waitingListIdentities)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        if (!waitingListIdentities.Any(w => w.Id == waitingListId))
        {
            return Result.Fail("Warteliste wurde nicht gefunden.");
        }

        var audit = new AuditMetadata(createdDate, createdBy);
        return Result.Ok(new PatientFile(waitingListId, audit, person));
    }
}
