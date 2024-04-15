namespace GtPrax.Application.UseCases.WaitingLists;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.UseCases.PatientFiles;
using Mediator;

internal sealed class GetWaitingListIndexHandler : IQueryHandler<GetWaitingListIndexQuery, WaitingListIndexDto[]>
{
    private readonly IWaitingListStore _waitingListStore;
    private readonly IPatientFileStore _patientFileStore;

    public GetWaitingListIndexHandler(
        IWaitingListStore waitingListStore,
        IPatientFileStore patientFileStore)
    {
        _waitingListStore = waitingListStore;
        _patientFileStore = patientFileStore;
    }

    public async ValueTask<WaitingListIndexDto[]> Handle(GetWaitingListIndexQuery query, CancellationToken cancellationToken)
    {
        var waitingLists = await _waitingListStore.GetAll(cancellationToken);
        if (waitingLists.Length < 1)
        {
            return [];
        }

        var patientCount = await _patientFileStore.GetCountByWaitingList(cancellationToken);

        return waitingLists
            .Select(w => new WaitingListIndexDto(w.Identity.Id!, w.Identity.Name, patientCount.TryGetValue(w.Identity.Id!, out var count) ? count : 0))
            .ToArray();
    }
}
