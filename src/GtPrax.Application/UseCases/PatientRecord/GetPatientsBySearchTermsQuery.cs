namespace GtPrax.Application.UseCases.PatientRecord;

using FluentResults;
using Mediator;

public sealed record GetPatientsBySearchTermsQuery(string WaitingListItemId, string? SearchTerms) : IQuery<Result<PatientRecordIndexDto>>;
