namespace GtPrax.Domain.Entities;

using FluentResults;

public sealed class WaitingListBuilder
{
    private string? _id;
    private string? _name;
    private string? _createdBy;
    private DateTimeOffset? _createdDate;

    public WaitingListBuilder SetId(string id) => Set(() => _id = id);

    public WaitingListBuilder SetName(string name) => Set(() => _name = name);

    public WaitingListBuilder SetCreated(string createdBy, DateTimeOffset createdDate)
    {
        _createdBy = createdBy;
        _createdDate = createdDate;
        return this;
    }

    public Result<WaitingList> Build(WaitingListIdentity[] identities)
    {
        ArgumentNullException.ThrowIfNull(identities);

        if (string.IsNullOrWhiteSpace(_name))
        {
            return Result.Fail("Der Name der Warteliste wird benötigt.");
        }

        var exists = identities.Any(w => w.Name == _name);
        if (exists)
        {
            return Result.Fail("Die Warteliste existiert bereits.");
        }

        if (string.IsNullOrWhiteSpace(_createdBy) || _createdDate is null)
        {
            return Result.Fail("Ein Ersteller wird benötigt.");
        }

        var audit = new AuditMetadata(_createdBy, _createdDate!.Value, null, null);

        return Result.Ok(new WaitingList(new WaitingListIdentity(_id, _name), audit));
    }

    private WaitingListBuilder Set(Action action)
    {
        action();
        return this;
    }
}
