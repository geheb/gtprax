namespace GtPrax.Domain.Entities;

public sealed class Prescription
{
    public string? Reason { get; private set; }
    public string? Doctor { get; private set; }
}
