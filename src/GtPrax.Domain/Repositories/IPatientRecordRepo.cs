namespace GtPrax.Domain.Repositories;

using GtPrax.Domain.Models;

public interface IPatientRecordRepo
{
    Task Upsert(PatientRecord entity, CancellationToken cancellationToken);
    Task<PatientRecord?> Find(PatientRecordId id, CancellationToken cancellationToken);
    Task<PatientRecord[]> GetAll(CancellationToken cancellationToken);
}
