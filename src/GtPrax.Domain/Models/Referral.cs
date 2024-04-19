namespace GtPrax.Domain.Models;

public sealed class Referral
{
    public string? Reason { get; private set; }
    public string? Doctor { get; private set; }

    public Referral(string? reason, string? doctor)
    {
        Reason = reason;
        Doctor = doctor;
    }
}
