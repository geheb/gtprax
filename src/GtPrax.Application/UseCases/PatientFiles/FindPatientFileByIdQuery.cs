namespace GtPrax.Application.UseCases.PatientFiles;

using Mediator;

public sealed record FindPatientFileByIdQuery(string Id) : IQuery<PatientFileDto?>;
