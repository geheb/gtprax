namespace GtPrax.Application.UseCases.PatientRecord;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using GtPrax.Application.Converter;
using GtPrax.Application.Services;
using GtPrax.Domain.Repositories;
using GtPrax.Domain.ValueObjects;
using Mediator;

internal sealed class GetPatientsBySearchHandler : IQueryHandler<GetPatientsBySearchQuery, PatientRecordIndexDto>
{
    private readonly TimeProvider _timeProvider;
    private readonly IUserService _userService;
    private readonly IWaitingListRepo _waitingListRepo;
    private readonly IPatientRecordRepo _patientRecordRepo;

    public GetPatientsBySearchHandler(
        TimeProvider timeProvider,
        IUserService userService,
        IWaitingListRepo waitingListRepo,
        IPatientRecordRepo patientRecordRepo)
    {
        _timeProvider = timeProvider;
        _userService = userService;
        _waitingListRepo = waitingListRepo;
        _patientRecordRepo = patientRecordRepo;
    }

    public async ValueTask<PatientRecordIndexDto> Handle(GetPatientsBySearchQuery query, CancellationToken cancellationToken)
    {
        var waitingListItems = await _waitingListRepo.GetAll(cancellationToken);
        var patientRecords = await _patientRecordRepo.GetAll(cancellationToken);

        var waitingList = new Domain.Models.WaitingList(waitingListItems, patientRecords);
        var localNow = new GermanDateTimeConverter().ToLocal(_timeProvider.GetUtcNow()).DateTime;
        var filter = query.Filter != PatientRecordFilter.None ? FilterType.From((int)query.Filter) : null;

        var (items, totalCount) = waitingList.FindPatients(query.WaitingListItemId, query.SearchTerms, filter, localNow);
        if (items.Length < 1)
        {
            return new([], totalCount);
        }

        var users = await _userService.GetAll(cancellationToken);

        var userMap = users.ToDictionary(u => u.Id);
        var dateTimeConverter = new GermanDateTimeConverter();
        var mappedItems = items.Select(p =>
            p.MapToIndexDto(userMap.TryGetValue(p.Audit.LastModifiedById ?? p.Audit.CreatedById, out var user) ? user.Name : null, dateTimeConverter)).ToArray();

        return new(mappedItems, totalCount);
    }
}
