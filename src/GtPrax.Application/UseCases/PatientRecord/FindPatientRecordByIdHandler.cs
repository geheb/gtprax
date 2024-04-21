namespace GtPrax.Application.UseCases.PatientRecord;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Domain.Repositories;
using Mediator;

internal sealed class FindPatientRecordByIdHandler : IQueryHandler<FindPatientRecordByIdQuery, PatientRecordDto?>
{
    private readonly IPatientRecordRepo _patientRecordRepo;

    public FindPatientRecordByIdHandler(IPatientRecordRepo patientRecordRepo)
    {
        _patientRecordRepo = patientRecordRepo;
    }

    public async ValueTask<PatientRecordDto?> Handle(FindPatientRecordByIdQuery query, CancellationToken cancellationToken)
    {
        var patientFile = await _patientRecordRepo.Find(new(query.Id), cancellationToken);
        return patientFile?.MapToDto();
    }
}
