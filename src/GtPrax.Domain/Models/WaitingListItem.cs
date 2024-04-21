namespace GtPrax.Domain.Models;

public sealed class WaitingListItem : Entity<WaitingListItemId>
{
    public string Name { get; private set; }
    public AuditMetadata Audit { get; private set; }

    internal WaitingListItem(WaitingListItemId id, string name, string createdBy, DateTimeOffset createOn)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        Name = name;
        Audit = new(createdBy, createOn);
    }
}
