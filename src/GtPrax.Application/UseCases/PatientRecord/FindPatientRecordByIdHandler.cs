namespace GtPrax.Application.UseCases.PatientRecord;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.Converter;
using GtPrax.Application.Services;
using GtPrax.Domain.Repositories;
using Mediator;

internal sealed class FindPatientRecordByIdHandler : IQueryHandler<FindPatientRecordByIdQuery, PatientRecordDto?>
{
    private readonly IUserService _userService;
    private readonly IWaitingListRepo _waitingListRepo;
    private readonly IPatientRecordRepo _patientRecordRepo;

    public FindPatientRecordByIdHandler(
        IUserService userService,
        IWaitingListRepo waitingListRepo,
        IPatientRecordRepo patientRecordRepo)
    {
        _userService = userService;
        _waitingListRepo = waitingListRepo;
        _patientRecordRepo = patientRecordRepo;
    }

    public async ValueTask<PatientRecordDto?> Handle(FindPatientRecordByIdQuery query, CancellationToken cancellationToken)
    {
        var patient = await _patientRecordRepo.Find(query.Id, cancellationToken);
        if (patient is null)
        {
            return null;
        }
        var waitingList = await _waitingListRepo.Find(patient.WaitingListItemId, cancellationToken);
        if (waitingList is null)
        {
            return null;
        }
        var users = await _userService.GetAll(cancellationToken);
        var userMap = users.ToDictionary(u => u.Id);
        var lastModifiedBy = userMap.TryGetValue(patient.Audit.LastModifiedById ?? patient.Audit.CreatedById, out var user) ? user.Name : null;
        return new(waitingList.Id, waitingList.Name, patient.MapToDto(lastModifiedBy, new GermanDateTimeConverter()));
    }
}
