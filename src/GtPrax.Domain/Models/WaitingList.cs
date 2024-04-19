namespace GtPrax.Domain.Models;

using FluentResults;

public sealed class WaitingList
{
    private readonly HashSet<WaitingListItem> _items;
    public IReadOnlyCollection<WaitingListItem> Items => _items;

    public WaitingList(IEnumerable<WaitingListItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items = new(items);
    }

    public Result<WaitingListItem> AddItem(string id, string name, string createdBy, DateTimeOffset createdOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);

        var exists = _items.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            return Result.Fail("Warteliste existiert bereits.");
        }

        var item = new WaitingListItem(new WaitingListItemId(id), name, createdBy, createdOn, []);
        _items.Add(item);

        return item;
    }

    public Result<WaitingListItem> AddItem(string name, string createdBy, DateTimeOffset createdOn) =>
        AddItem(DomainObjectId.NewId(), name, createdBy, createdOn);
}
