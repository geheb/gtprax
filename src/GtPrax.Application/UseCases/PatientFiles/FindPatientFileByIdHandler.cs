namespace GtPrax.Application.UseCases.PatientFiles;

using System.Threading;
using System.Threading.Tasks;
using Mediator;

internal sealed class FindPatientFileByIdHandler : IQueryHandler<FindPatientFileByIdQuery, PatientFileDto?>
{
    private readonly IPatientFileStore _patientFileStore;

    public FindPatientFileByIdHandler(IPatientFileStore patientFileStore)
    {
        _patientFileStore = patientFileStore;
    }

    public async ValueTask<PatientFileDto?> Handle(FindPatientFileByIdQuery query, CancellationToken cancellationToken)
    {
        var patientFile = await _patientFileStore.Find(query.Id, cancellationToken);
        return patientFile?.MapToDto();
    }
}
