namespace GtPrax.Application.UseCases.PatientRecord;

using FluentResults;
using Mediator;

public sealed record GetPatientRecordIndexQuery(string WaitingListItemId) : IQuery<Result<PatientRecordIndexDto>>;
