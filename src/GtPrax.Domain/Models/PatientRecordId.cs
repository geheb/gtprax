namespace GtPrax.Domain.Models;

public sealed class PatientRecordId : DomainObjectId
{
    public PatientRecordId(string id) : base(id)
    {
    }

    public static implicit operator string(PatientRecordId id) => id.Value;
    public static implicit operator PatientRecordId(string id) => new(id);
}
