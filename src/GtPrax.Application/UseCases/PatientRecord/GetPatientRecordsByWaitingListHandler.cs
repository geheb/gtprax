namespace GtPrax.Application.UseCases.PatientRecord;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Domain.Repositories;
using Mediator;

internal sealed class GetPatientRecordsByWaitingListHandler : IQueryHandler<GetPatientRecordsByWaitingListQuery, Result<PatientRecordDto[]>>
{
    private readonly IWaitingListRepo _waitingListRepo;
    private readonly IPatientRecordRepo _patientRecordRepo;

    public GetPatientRecordsByWaitingListHandler(
        IWaitingListRepo waitingListRepo,
        IPatientRecordRepo patientRecordRepo)
    {
        _waitingListRepo = waitingListRepo;
        _patientRecordRepo = patientRecordRepo;
    }

    public async ValueTask<Result<PatientRecordDto[]>> Handle(GetPatientRecordsByWaitingListQuery query, CancellationToken cancellationToken)
    {
        var waitingListItems = await _waitingListRepo.GetAll(cancellationToken);
        var patientRecords = await _patientRecordRepo.GetAll(cancellationToken);

        var result = new Domain.Models.WaitingList(waitingListItems, patientRecords)
            .GetPatientsByWaitingList(query.WaitingListItemId);

        if (result.IsFailed)
        {
            return result.ToResult();
        }

        return Result.Ok(result.Value.MapToDto());
    }
}
