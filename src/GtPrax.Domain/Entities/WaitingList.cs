namespace GtPrax.Domain.Entities;

using FluentResults;

public sealed class WaitingList
{
    private readonly List<PatientFile> _patientFiles = [];

    public WaitingListIdentity Identity { get; private set; }
    public IReadOnlyCollection<PatientFile> PatientFiles => _patientFiles;

    public WaitingList(WaitingListIdentity identity)
    {
        Identity = identity;
    }

    public static Result<WaitingList> Create(string name, WaitingListIdentity[] identities)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var exists = identities.Any(w => w.Name == name);
        if (exists)
        {
            return Result.Fail("Die Warteliste existiert bereits.");
        }

        return Result.Ok(new WaitingList(new WaitingListIdentity(string.Empty, name)));
    }

    public Result Add(PatientFile patientFile, PersonIdentity[] identities)
    {
        ArgumentNullException.ThrowIfNull(patientFile);
        var exists = identities.Any(p => p.Name == patientFile.Person.Identity.Name && p.BirthDate == patientFile.Person.Identity.BirthDate);
        if (exists)
        {
            return Result.Fail("Der Patient existiert bereits.");
        }
        _patientFiles.Add(patientFile);

        return Result.Ok();
    }
}
