namespace GtPrax.Application.UseCases.PatientRecord;

using Mediator;

public sealed record FindPatientRecordByIdQuery(string Id) : IQuery<PatientRecordDto?>;
