namespace GtPrax.Application.UseCases.WaitingList;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Domain.Repositories;
using Mediator;

internal sealed class GetWaitingListIndexHandler : IQueryHandler<GetWaitingListIndexQuery, WaitingListIndexDto[]>
{
    private readonly IWaitingListRepo _waitingListRepo;
    private readonly IPatientRecordRepo _patientRecordRepo;

    public GetWaitingListIndexHandler(
        IWaitingListRepo waitingListRepo,
        IPatientRecordRepo patientRecordRepo)
    {
        _waitingListRepo = waitingListRepo;
        _patientRecordRepo = patientRecordRepo;
    }

    public async ValueTask<WaitingListIndexDto[]> Handle(GetWaitingListIndexQuery query, CancellationToken cancellationToken)
    {
        var waitingLists = await _waitingListRepo.GetAll(cancellationToken);
        if (waitingLists.Length < 1)
        {
            return [];
        }

        var patientRecords = await _patientRecordRepo.GetAll(cancellationToken);

        var waitingList = new Domain.Models.WaitingList(waitingLists, patientRecords);
        return waitingList.GetPatientsGroupedByWaitingList().Select(w => new WaitingListIndexDto(w.Item.Id, w.Item.Name, w.PatientCount)).ToArray();
    }
}
