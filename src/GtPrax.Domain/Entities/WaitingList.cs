namespace GtPrax.Domain.Entities;

public sealed class WaitingList
{
    public WaitingListIdentity Identity { get; private set; }
    public AuditMetadata Audit { get; private set; }

    internal WaitingList(WaitingListIdentity identity, AuditMetadata audit)
    {
        Identity = identity;
        Audit = audit;
    }
}
