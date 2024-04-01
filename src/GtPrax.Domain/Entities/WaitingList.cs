namespace GtPrax.Domain.Entities;

using FluentResults;

public sealed class WaitingList
{
    public WaitingListIdentity Identity { get; private set; }

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
}
