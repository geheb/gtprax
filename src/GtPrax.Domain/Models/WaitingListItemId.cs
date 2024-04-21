namespace GtPrax.Domain.Models;


public sealed class WaitingListItemId : DomainObjectId
{
    public WaitingListItemId(string id) : base(id)
    {
    }

    public static implicit operator string(WaitingListItemId id) => id.Value;
    public static implicit operator WaitingListItemId(string id) => new(id);
}
