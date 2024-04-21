namespace GtPrax.Domain.Models;

using FluentResults;
using GtPrax.Domain.ValueObjects;

public sealed class WaitingList
{
    private readonly HashSet<WaitingListItem> _waitingListItems;
    private readonly HashSet<PatientRecord> _patientRecords;

    public WaitingList(IEnumerable<WaitingListItem> waitingListItems, IEnumerable<PatientRecord> patientRecords)
    {
        ArgumentNullException.ThrowIfNull(waitingListItems);
        _waitingListItems = new(waitingListItems);
        _patientRecords = new(patientRecords);
    }

    public Result<WaitingListItem> AddWaitingList(string name, string createdBy, DateTimeOffset createdOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        var exists = _waitingListItems.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            return Result.Fail("Die Warteliste existiert bereits.");
        }

        var item = new WaitingListItem(DomainObjectId.NewId(), name, createdBy, createdOn);
        _waitingListItems.Add(item);

        return item;
    }

    public Result<PatientRecord[]> GetPatientsByWaitingList(WaitingListItemId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (!_waitingListItems.Any(w => w.Id == id))
        {
            return Result.Fail("Die Warteliste wurde nicht gefunden.");
        }
        return Result.Ok(_patientRecords.Where(p => p.WaitingListItemId == id).ToArray());
    }

    public IEnumerable<(WaitingListItem Item, int PatientCount)> GetPatientsGroupedByWaitingList()
    {
        var map = _patientRecords.GroupBy(p => p.WaitingListItemId).ToDictionary(g => g.Key, g => g.Count());
        return _waitingListItems.Select(w => (w, map.TryGetValue(w.Id, out var count) ? count : 0)).ToArray();
    }

    public Result<PatientRecord> AddPatient(
        WaitingListItemId id,
        string createdBy,
        DateTimeOffset createdOn,
        Person person,
        Referral referral,
        IReadOnlyDictionary<DayOfWeek, TherapyDay> therapyDays,
        IEnumerable<TagType> tags,
        string? remark)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);
        ArgumentNullException.ThrowIfNull(person);
        ArgumentNullException.ThrowIfNull(therapyDays);
        ArgumentNullException.ThrowIfNull(tags);

        var waitingList = _waitingListItems.FirstOrDefault(w => w.Id == id);
        if (waitingList is null)
        {
            return Result.Fail("Die Warteliste wurde nicht gefunden.");
        }

        var existentPatient = _patientRecords.FirstOrDefault(p =>
            p.Person.Name.Equals(person.Name, StringComparison.OrdinalIgnoreCase) && p.Person.BirthDate == person.BirthDate);

        if (existentPatient is not null)
        {
            if (existentPatient.WaitingListItemId == id)
            {
                return Result.Fail("Der/die Patient(in) mit Name und Geburtsdatum existiert bereits.");
            }
            return Result.Fail($"Der/die Patient(in) mit Name und Geburtsdatum existiert bereits. Siehe Warteliste \"{waitingList.Name}\"");
        }

        var audit = new AuditMetadata(createdBy, createdOn);
        var record = new PatientRecord(DomainObjectId.NewId(), id, audit, person, referral, therapyDays, tags, remark);

        _patientRecords.Add(record);

        return Result.Ok(record);
    }
}
