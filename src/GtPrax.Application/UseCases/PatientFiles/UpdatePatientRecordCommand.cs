namespace GtPrax.Application.UseCases.PatientFiles;

using FluentResults;
using Mediator;

public sealed record UpdatePatientRecordCommand(string Id, string ModifiedBy, UpdatePatientRecordDto PatientRecord) : ICommand<Result>;
