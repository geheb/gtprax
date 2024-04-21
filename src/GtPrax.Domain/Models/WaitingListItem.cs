namespace GtPrax.Domain.Models;

public sealed class WaitingListItem : Entity<WaitingListItemId>
{
    public string Name { get; private set; }
    public AuditMetadata Audit { get; private set; }

    internal WaitingListItem(WaitingListItemId id, string name, string createdById, DateTimeOffset createOn)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdById);

        Name = name;
        Audit = new(createdById, createOn);
    }
}
