namespace GtPrax.Application.UseCases.PatientFiles;

using GtPrax.Domain.Entities;

public interface IPatientFileStore
{
    Task Upsert(PatientFile entity, CancellationToken cancellationToken);
    Task<PatientFile?> Find(string id, CancellationToken cancellationToken);
    Task<PersonIdentity[]> GetIdentities(CancellationToken cancellationToken);
    Task<Dictionary<string, int>> GetCountByWaitingList(CancellationToken cancellationToken);
}
