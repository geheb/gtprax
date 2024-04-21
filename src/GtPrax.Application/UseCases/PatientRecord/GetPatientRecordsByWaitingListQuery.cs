namespace GtPrax.Application.UseCases.PatientRecord;

using FluentResults;
using Mediator;

public sealed record GetPatientRecordsByWaitingListQuery(string WaitingListItemId) : IQuery<Result<PatientRecordDto[]>>;
