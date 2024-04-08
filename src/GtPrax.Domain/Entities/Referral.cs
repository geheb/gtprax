namespace GtPrax.Domain.Entities;

public sealed class Referral
{
    public string? Reason { get; private set; }
    public string? Doctor { get; private set; }

    internal Referral(string? reason, string? doctor)
    {
        Reason = reason;
        Doctor = doctor;
    }
}
