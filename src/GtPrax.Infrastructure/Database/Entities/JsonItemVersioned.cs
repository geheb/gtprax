namespace GtPrax.Infrastructure.Database.Entities;

internal sealed class JsonItemVersioned<T>
{
    public int Version { get; set; }
    public T? Item { get; set; }
}
