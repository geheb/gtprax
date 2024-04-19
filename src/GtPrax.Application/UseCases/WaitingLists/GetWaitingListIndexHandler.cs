namespace GtPrax.Application.UseCases.WaitingLists;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Domain.Repositories;
using GtPrax.Domain.ValueObjects;
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
        var patientRecordMap = patientRecords.GroupBy(p => p.WaitingListItemId.Value).ToDictionary(g => g.Key, g => g.Count());

        return waitingLists
            .Select(w => new WaitingListIndexDto(w.Id.Value, w.Name, patientRecordMap.TryGetValue(w.Id.Value, out var count) ? count : 0))
            .ToArray();
    }
}
