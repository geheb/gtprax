namespace GtPrax.Domain.Models;

using System.Globalization;
using System.Text.RegularExpressions;
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

    public Result<WaitingListItem> AddWaitingList(string name, string createdById, DateTimeOffset createdOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdById);

        var exists = _waitingListItems.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            return Result.Fail("Die Warteliste existiert bereits.");
        }

        var item = new WaitingListItem(DomainObjectId.NewId(), name, createdById, createdOn);
        _waitingListItems.Add(item);

        return item;
    }

    public IEnumerable<(WaitingListItem Item, int PatientCount)> GetWaitingListsGroupedByPatient()
    {
        var map = _patientRecords.GroupBy(p => p.WaitingListItemId).ToDictionary(g => g.Key, g => g.Count());
        return _waitingListItems.Select(w => (w, map.TryGetValue(w.Id, out var count) ? count : 0)).ToArray();
    }

    public WaitingListItem? FindWaitingList(WaitingListItemId id) => _waitingListItems.FirstOrDefault(w => w.Id == id);

    public Result<PatientRecord> AddPatient(
        WaitingListItemId id,
        string createdById,
        DateTimeOffset createdOn,
        Person person,
        Referral referral,
        IReadOnlyDictionary<DayOfWeek, TherapyDay> therapyDays,
        IEnumerable<TagType> tags,
        string? remark)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdById);
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

        var audit = new AuditMetadata(createdById, createdOn);
        var record = new PatientRecord(DomainObjectId.NewId(), id, audit, person, referral, therapyDays, tags, remark);

        _patientRecords.Add(record);

        return Result.Ok(record);
    }

    public Result<PatientRecord[]> FindPatients(WaitingListItemId id, string? searchTerms)
    {
        ArgumentNullException.ThrowIfNull(id);

        var waitingList = _waitingListItems.FirstOrDefault(w => w.Id == id);
        if (waitingList is null)
        {
            return Result.Fail("Die Warteliste wurde nicht gefunden.");
        }

        if (string.IsNullOrWhiteSpace(searchTerms))
        {
            return Result.Ok(_patientRecords.Where(p => p.WaitingListItemId == id).ToArray());
        }

        (var date, searchTerms) = ExtractDate(searchTerms);
        (var numbers, searchTerms) = ExtractNumberSequence(searchTerms);
        var words = ExtractWords(searchTerms);

        if (date is null && numbers is null && words.Length < 1)
        {
            return Result.Ok(_patientRecords.Where(p => p.WaitingListItemId == id).ToArray());
        }

        var result = _patientRecords
            .Where(p =>
                (date is null || p.Person.BirthDate == date) &&
                (numbers is null || p.Person.PhoneNumber.Contains(numbers)) &&
                (words.Length == 0 || words.All(w => p.Person.Name.Contains(w, StringComparison.OrdinalIgnoreCase))))
            .OrderBy(p => p.Audit.CreatedDate)
            .ToArray();

        if (result.Length > 0 && result.All(r => r.WaitingListItemId != id))
        {
            var waitingListsIds = result.Select(r => r.WaitingListItemId).Distinct().ToArray();
            var waitingListNames = _waitingListItems.Where(w => waitingListsIds.Contains(w.Id)).Select(w => w.Name).ToArray();
            return Result.Fail("Die Suchergebnisse sind in folgenden Wartelisten vorhanden: " + string.Join(", ", waitingListNames));
        }

        return result;
    }

    private static string[] ExtractWords(string searchTerms) => searchTerms.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    private static (string?, string) ExtractNumberSequence(string searchTerms)
    {
        var match = Regex.Match(searchTerms, "(\\d+)");
        if (!match.Success)
        {
            return (null, searchTerms);
        }
        var cleanedSearch = searchTerms.Remove(match.Index, match.Length);
        return (match.Value, cleanedSearch);
    }

    private static (DateOnly?, string) ExtractDate(string searchTerms)
    {
        var match = Regex.Match(searchTerms, "(\\d{1,2}\\.\\d{1,2}\\.\\d{4})");
        if (!match.Success)
        {
            return (null, searchTerms);
        }

        var splitDate = match.Groups[0].Value.Split('.');
        var paddingDate = splitDate[0].PadLeft(2, '0') + '.' + splitDate[1].PadLeft(2, '0') + '.' + splitDate[2];
        if (!DateOnly.TryParseExact(paddingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return (null, searchTerms);
        }
        var cleanedSearch = searchTerms.Remove(match.Index, match.Length);
        return (date, cleanedSearch);
    }
}
