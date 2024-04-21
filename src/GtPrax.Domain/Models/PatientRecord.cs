namespace GtPrax.Domain.Models;

using System;
using GtPrax.Domain.ValueObjects;

public sealed class PatientRecord : Entity<PatientRecordId>
{
    private readonly Dictionary<DayOfWeek, TherapyDay> _therapyDays;
    private readonly HashSet<TagType> _tags;

    public WaitingListItemId WaitingListItemId { get; private set; }
    public AuditMetadata Audit { get; private set; }
    public Person Person { get; private set; }
    public Referral? Referral { get; private set; }
    public IReadOnlyDictionary<DayOfWeek, TherapyDay> TherapyDays => _therapyDays;
    public IReadOnlyCollection<TagType> Tags => _tags;
    public string? Remark { get; private set; }

    internal PatientRecord(
        PatientRecordId id,
        WaitingListItemId waitingListItemId,
        AuditMetadata audit,
        Person person,
        Referral? referral,
        IReadOnlyDictionary<DayOfWeek, TherapyDay> therapyDays,
        IEnumerable<TagType> tags,
        string? remark) : base(id)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(waitingListItemId);
        ArgumentNullException.ThrowIfNull(audit);
        ArgumentNullException.ThrowIfNull(person);
        ArgumentNullException.ThrowIfNull(therapyDays);
        ArgumentNullException.ThrowIfNull(tags);

        WaitingListItemId = waitingListItemId;
        Audit = audit;
        Person = person;
        Referral = referral;
        _therapyDays = new(therapyDays);
        _tags = new(tags);
        Remark = remark;
    }

    public PatientRecord Update(
        string modifiedById,
        DateTimeOffset modifiedOn,
        string phoneNumber,
        Referral referral,
        IReadOnlyDictionary<DayOfWeek, TherapyDay> therapyDays,
        IEnumerable<TagType> tags,
        string? remark) =>
        new(Id,
            WaitingListItemId,
            new(Audit.CreatedById, Audit.CreatedDate, modifiedById, modifiedOn),
            Person.SetPhoneNumber(phoneNumber),
            referral,
            therapyDays,
            tags,
            remark);
}
