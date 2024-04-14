namespace GtPrax.Domain.Entities;

using FluentResults;
using GtPrax.Domain.ValueObjects;

public sealed class PatientFileBuilder
{
    private string? _id;
    private string? _waitingListId;
    private string? _createdBy;
    private DateTimeOffset? _createdDate;
    private string? _lastModifiedBy;
    private DateTimeOffset? _lastModifiedDate;
    private Person? _person;
    private Referral? _referral;
    private readonly Dictionary<DayOfWeek, TherapyDay> _therapyDays = [];
    private readonly HashSet<TagType> _tags = [];
    private string? _remark;

    public PatientFileBuilder()
    {
    }

    public PatientFileBuilder(PatientFile patientFile)
    {
        _id = patientFile.Id;
        _waitingListId = patientFile.WaitingListId;
        _createdBy = patientFile.Audit.CreatedBy;
        _createdDate = patientFile.Audit.CreatedDate;
        _lastModifiedBy = patientFile.Audit.LastModifiedBy;
        _lastModifiedDate = patientFile.Audit.LastModifiedDate;
        _person = patientFile.Person;
        _referral = patientFile.Referral;
        _therapyDays = new(patientFile.TherapyDays);
        _tags = new(patientFile.Tags);
        _remark = patientFile.Remark;
    }

    public PatientFileBuilder SetId(string id) => Set(() => _id = id);

    public PatientFileBuilder SetWaitingListId(string id) => Set(() => _waitingListId = id);

    public PatientFileBuilder SetCreated(string createdBy, DateTimeOffset createdDate)
    {
        _createdBy = createdBy;
        _createdDate = createdDate;
        return this;
    }

    public PatientFileBuilder SetLastModified(string? lastModifiedBy, DateTimeOffset? lastModifiedDate)
    {
        _lastModifiedBy = lastModifiedBy;
        _lastModifiedDate = lastModifiedDate;
        return this;
    }

    public PatientFileBuilder SetPerson(Person person) => Set(() => _person = person);

    public PatientFileBuilder SetReferral(string? reason, string? doctor)
    {
        if (reason is null && doctor is null)
        {
            _referral = null;
        }
        _referral = new(reason, doctor);
        return this;
    }

    public PatientFileBuilder SetTherapyDay(DayOfWeek day, bool isMorning, bool isAfternoon, bool isHomeVisit, TimeOnly? availableFrom) =>
        Set(() => _therapyDays[day] = new TherapyDay(isMorning, isAfternoon, isHomeVisit, availableFrom));

    public PatientFileBuilder SetTag(TagType tag) => Set(() => _tags.Add(tag));

    public PatientFileBuilder SetRemark(string? remark) => Set(() => _remark = remark);


    public Result<PatientFile> Build(WaitingListIdentity[] identities)
    {
        ArgumentNullException.ThrowIfNull(identities);

        if (string.IsNullOrWhiteSpace(_waitingListId))
        {
            return Result.Fail("Eine Warteliste wird benötigt.");
        }

        if (!identities.Any(w => w.Id == _waitingListId))
        {
            return Result.Fail("Die Warteliste wurde nicht gefunden.");
        }

        if (_person is null)
        {
            return Result.Fail("Eine Person wird benötigt.");
        }

        if (string.IsNullOrWhiteSpace(_createdBy) || _createdDate is null)
        {
            return Result.Fail("Ein Ersteller wird benötigt.");
        }

        if (_lastModifiedDate is not null && _lastModifiedDate < _createdDate)
        {
            return Result.Fail("Das Änderungsdatum ist ungültig.");
        }

        var audit = new AuditMetadata(_createdBy, _createdDate!.Value, _lastModifiedBy, _lastModifiedDate);

        var patientFile = new PatientFile(_id, _waitingListId!, audit, _person, _referral, _therapyDays, _tags, _remark);
        return Result.Ok(patientFile);
    }

    private PatientFileBuilder Set(Action action)
    {
        action();
        return this;
    }
}
