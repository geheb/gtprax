namespace GtPrax.Application.Repositories;

using GtPrax.Application.Models;

public interface IWaitlistRepository
{
    Task<WaitlistDto[]> GetAll(CancellationToken cancellationToken);
    Task<WaitlistDto?> Find(Guid id, CancellationToken cancellationToken);
    Task<bool> Create(string name, CancellationToken cancellationToken);
    Task<WaitlistPatientDto?> FindPatient(Guid id, CancellationToken cancellationToken);
    Task<WaitlistPatientDto[]> GetPatients(Guid? waitlistId, string? searchKey, CancellationToken cancellationToken);
    Task<bool> Update(WaitlistPatientDto dto, CancellationToken cancellationToken);
    Task<bool> DeletePatient(Guid id, Guid waitlistId, CancellationToken cancellationToken);
    Task<bool> MovePatient(Guid id, Guid waitlistId, CancellationToken cancellationToken);
    Task<bool> CreatePatient(WaitlistPatientDto dto, CancellationToken cancellationToken);
}
