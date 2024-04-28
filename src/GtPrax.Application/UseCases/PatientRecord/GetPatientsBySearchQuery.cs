namespace GtPrax.Application.UseCases.PatientRecord;

using Mediator;

public sealed record GetPatientsBySearchQuery(string WaitingListItemId, string? SearchTerms, PatientRecordFilter Filter) : IQuery<PatientRecordIndexDto>;
