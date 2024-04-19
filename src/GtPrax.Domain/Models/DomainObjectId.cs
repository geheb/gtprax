namespace GtPrax.Domain.Models;

using GtPrax.Domain.ValueObjects;

public abstract class DomainObjectId : ValueObject
{
    public string Value { get; }

    protected DomainObjectId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Length != 24)
        {
            throw new ArgumentException("Invalid length", nameof(value));
        }

        Value = value;
    }

    public static string NewId()
    {
        var timestamp = BitConverter.GetBytes((int)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds);
        var random = Guid.NewGuid().ToByteArray()[..8];
        return Convert.ToHexString([.. timestamp, .. random]).ToLowerInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
