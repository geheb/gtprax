namespace GtPrax.Application.UseCases.PatientRecord;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Converter;
using GtPrax.Application.Services;
using GtPrax.Domain.Repositories;
using Mediator;

internal sealed class GetPatientsBySearchTermsHandler : IQueryHandler<GetPatientsBySearchTermsQuery, Result<PatientRecordIndexDto>>
{
    private readonly IUserService _userService;
    private readonly IWaitingListRepo _waitingListRepo;
    private readonly IPatientRecordRepo _patientRecordRepo;

    public GetPatientsBySearchTermsHandler(
        IUserService userService,
        IWaitingListRepo waitingListRepo,
        IPatientRecordRepo patientRecordRepo)
    {
        _userService = userService;
        _waitingListRepo = waitingListRepo;
        _patientRecordRepo = patientRecordRepo;
    }

    public async ValueTask<Result<PatientRecordIndexDto>> Handle(GetPatientsBySearchTermsQuery query, CancellationToken cancellationToken)
    {
        var waitingListItems = await _waitingListRepo.GetAll(cancellationToken);
        var patientRecords = await _patientRecordRepo.GetAll(cancellationToken);

        var waitingList = new Domain.Models.WaitingList(waitingListItems, patientRecords);
        var result = waitingList.FindPatients(query.WaitingListItemId, query.SearchTerms);
        if (result.IsFailed)
        {
            return result.ToResult();
        }

        var waitingListName = waitingList.FindWaitingList(query.WaitingListItemId)!.Name;

        if (result.Value.Length < 1)
        {
            return Result.Ok(new PatientRecordIndexDto(waitingListName, []));
        }

        var users = await _userService.GetAll(cancellationToken);

        var userMap = users.ToDictionary(u => u.Id);
        var dateTimeConverter = new GermanDateTimeConverter();
        var items = result.Value.Select(p =>
            p.MapToIndexDto(userMap.TryGetValue(p.Audit.LastModifiedById ?? p.Audit.CreatedById, out var user) ? user.Name : null, dateTimeConverter)).ToArray();

        return Result.Ok(new PatientRecordIndexDto(waitingListName, items));
    }
}
