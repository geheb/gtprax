namespace GtPrax.Domain.Entities;

using System;
using GtPrax.Domain.ValueObjects;

public sealed class PatientFile
{
    private readonly Dictionary<DayOfWeek, TherapyDay> _therapyDays = [];
    private readonly HashSet<TagType> _tags = [];

    public string? Id { get; private set; }
    public string WaitingListId { get; private set; }
    public AuditMetadata Audit { get; private set; }
    public Person Person { get; private set; }
    public Referral? Referral { get; private set; }
    public IReadOnlyDictionary<DayOfWeek, TherapyDay> TherapyDays => _therapyDays;
    public IReadOnlyCollection<TagType> Tags => _tags;
    public string? Remark { get; private set; }

    internal PatientFile(
        string? id,
        string waitingListId,
        AuditMetadata audit,
        Person person,
        Referral? referral,
        IReadOnlyDictionary<DayOfWeek, TherapyDay> therapyDays,
        IEnumerable<TagType> tags,
        string? remark)
    {
        Id = id;
        WaitingListId = waitingListId;
        Audit = audit;
        Person = person;
        Referral = referral;
        _therapyDays = new(therapyDays);
        _tags = new(tags);
        Remark = remark;
    }
}
