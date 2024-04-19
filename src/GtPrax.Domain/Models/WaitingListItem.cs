namespace GtPrax.Domain.Models;

using FluentResults;
using GtPrax.Domain.ValueObjects;

public sealed class WaitingListItem : Entity<WaitingListItemId>
{
    private readonly HashSet<PatientRecord> _patientRecords;

    public string Name { get; private set; }
    public AuditMetadata Audit { get; private set; }
    public IReadOnlyCollection<PatientRecord> PatientRecords => _patientRecords;

    internal WaitingListItem(WaitingListItemId id, string name, string createdBy, DateTimeOffset createOn, IEnumerable<PatientRecord> patientRecords)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);
        ArgumentNullException.ThrowIfNull(patientRecords);

        _patientRecords = new(patientRecords);

        Name = name;
        Audit = new(createdBy, createOn);
    }

    public WaitingListItem AddPatientRecords(IEnumerable<PatientRecord> patientRecords) =>
        new(Id, Name, Audit.CreatedBy, Audit.CreatedDate, patientRecords);

    public Result<PatientRecord> AddPatientRecord(
        string createdBy,
        DateTimeOffset createdOn,
        Person person,
        Referral referral,
        IReadOnlyDictionary<DayOfWeek, TherapyDay> therapyDays,
        IEnumerable<TagType> tags,
        string? remark)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);
        ArgumentNullException.ThrowIfNull(person);
        ArgumentNullException.ThrowIfNull(therapyDays);
        ArgumentNullException.ThrowIfNull(tags);

        var exists = _patientRecords.Any(p => p.Person.Name.Equals(person.Name, StringComparison.OrdinalIgnoreCase) && p.Person.BirthDate == person.BirthDate);
        if (exists)
        {
            return Result.Fail("Patient (Name & Geburtsdatum) existiert bereits.");
        }

        var audit = new AuditMetadata(createdBy, createdOn);
        var record = new PatientRecord(new(DomainObjectId.NewId()), Id, audit, person, referral, therapyDays, tags, remark);

        _patientRecords.Add(record);

        return Result.Ok(record);
    }
}
