namespace GtPrax.Domain.Models;

public abstract class Entity<T> where T : DomainObjectId
{
    public T Id { get; }

    protected Entity(T id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<T> other)
        {
            return false;
        }
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
}
