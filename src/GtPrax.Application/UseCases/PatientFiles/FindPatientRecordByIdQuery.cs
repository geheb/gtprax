namespace GtPrax.Application.UseCases.PatientFiles;

using Mediator;

public sealed record FindPatientRecordByIdQuery(string Id) : IQuery<PatientRecordDto?>;
