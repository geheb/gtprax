namespace GtPrax.Domain.Entities;

public sealed class WaitingListIdentity
{
    public string Id { get; set; }
    public string Name { get; set; }

    public WaitingListIdentity(string id, string name)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
    }
}
