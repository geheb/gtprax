namespace GtPrax.Infrastructure.Database.Entities;

internal sealed class JsonListVersioned<T>
{
    public int Version { get; set; }
    public T[]? Items { get; set; }
}
