namespace GtPrax.Application.UseCases.PatientFiles;

using GtPrax.Domain.Entities;

public interface IPatientFileStore
{
    Task Create(PatientFile entity, CancellationToken cancellationToken);
    Task<PersonIdentity[]> GetIdentities(CancellationToken cancellationToken);
}
