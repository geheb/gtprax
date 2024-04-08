namespace GtPrax.Domain.Entities;

public sealed class WaitingListIdentity
{
    public string Id { get; set; }
    public string Name { get; set; }

    public WaitingListIdentity(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
    }

    public WaitingListIdentity(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = string.Empty;
        Name = name;
    }
}
