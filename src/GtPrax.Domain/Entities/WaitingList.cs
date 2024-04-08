namespace GtPrax.Domain.Entities;

using FluentResults;

public sealed class WaitingList
{
    public WaitingListIdentity Identity { get; private set; }
    public AuditMetadata Audit { get; private set; }

    public WaitingList(WaitingListIdentity identity, AuditMetadata audit)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(audit);

        Identity = identity;
        Audit = audit;
    }

    public static Result<WaitingList> Create(string name, string createdBy, DateTimeOffset createdDate, WaitingListIdentity[] currentIdentities)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        var exists = currentIdentities.Any(w => w.Name == name);
        if (exists)
        {
            return Result.Fail("Die Warteliste existiert bereits.");
        }

        return Result.Ok(new WaitingList(new WaitingListIdentity(name), new AuditMetadata(createdDate, createdBy)));
    }
}
