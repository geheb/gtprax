namespace GtPrax.Domain.Models;

using System.Globalization;
using System.Linq;
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

    public (PatientRecord[] Items, int TotalCount) FindPatients(WaitingListItemId id, string? searchTerms, FilterType? filter, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (_patientRecords.Count < 1)
        {
            return ([], 0);
        }

        var result = _patientRecords.Where(r => r.WaitingListItemId == id).ToArray();

        if (string.IsNullOrWhiteSpace(searchTerms) && filter is null)
        {
            return (result, result.Length);
        }

        var searchResult = ApplySearchTerms(result, searchTerms);
        searchResult = ApplyFilter(searchResult, filter, now);

        return (searchResult.OrderBy(r => r.Audit.CreatedDate).ToArray(), result.Length);
    }

    private static IEnumerable<PatientRecord> ApplySearchTerms(IEnumerable<PatientRecord> result, string? searchTerms)
    {
        if (string.IsNullOrWhiteSpace(searchTerms))
        {
            return result;
        }

        (var date, searchTerms) = ExtractDate(searchTerms);
        (var numbers, searchTerms) = ExtractNumberSequence(searchTerms);
        var words = ExtractWords(searchTerms);

        if (date is null && numbers is null && words.Length < 1)
        {
            return result;
        }

        return result.Where(p =>
            (date is null || p.Person.BirthDate == date) &&
            (numbers is null || p.Person.PhoneNumber.Contains(numbers)) &&
            (words.Length == 0 || words.All(w => p.Person.Name.Contains(w, StringComparison.OrdinalIgnoreCase))));
    }

    private static IEnumerable<PatientRecord> ApplyFilter(IEnumerable<PatientRecord> result, FilterType? filter, DateTime now)
    {
        if (!result.Any() || filter is null)
        {
            return result;
        }

        if (filter == FilterType.ChildrenInTheMorning)
        {
            return result.Where(r => r.TherapyDaysHasMorning && r.Person.CalcAge(now) < 18);
        }
        if (filter == FilterType.ChildrenInTheAfternoon)
        {
            return result.Where(r => r.TherapyDaysHasAfternoon && r.Person.CalcAge(now) < 18);
        }
        if (filter == FilterType.AdultsInTheMorning)
        {
            return result.Where(r => r.TherapyDaysHasMorning && r.Person.CalcAge(now) >= 18);
        }
        if (filter == FilterType.AdultsInTheAfternoon)
        {
            return result.Where(r => r.TherapyDaysHasAfternoon && r.Person.CalcAge(now) >= 18);
        }
        if (filter == FilterType.TherapyDayWithHomeVisit)
        {
            return result.Where(r => r.TherapyDaysHasHomeVisit);
        }
        if (filter == FilterType.HasTagPriority)
        {
            return result.Where(r => r.Tags.Contains(TagType.Priority));
        }
        if (filter == FilterType.HasTagJumper)
        {
            return result.Where(r => r.Tags.Contains(TagType.Jumper));
        }
        if (filter == FilterType.HasTagNeurofeedback)
        {
            return result.Where(r => r.Tags.Contains(TagType.Neurofeedback));
        }
        if (filter == FilterType.HasTagSchool)
        {
            return result.Where(r => r.Tags.Contains(TagType.School));
        }

        throw new NotImplementedException($"Missing filter {filter}");
    }

    private static string[] ExtractWords(string searchTerms) => searchTerms.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    private static (string?, string) ExtractNumberSequence(string searchTerms)
    {
        var match = Regex.Match(searchTerms, "(\\d+)", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(500));
        if (!match.Success)
        {
            return (null, searchTerms);
        }
        var cleanedSearch = searchTerms.Remove(match.Index, match.Length);
        return (match.Value, cleanedSearch);
    }

    private static (DateOnly?, string) ExtractDate(string searchTerms)
    {
        var match = Regex.Match(searchTerms, "(\\d{1,2}\\.\\d{1,2}\\.\\d{4})", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(500));
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
