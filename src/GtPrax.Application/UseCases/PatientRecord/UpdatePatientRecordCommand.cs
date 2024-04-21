namespace GtPrax.Application.UseCases.PatientRecord;

using FluentResults;
using Mediator;

public sealed record UpdatePatientRecordCommand(string Id, string ModifiedBy, UpdatePatientRecordDto PatientRecord) : ICommand<Result>;
