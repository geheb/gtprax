namespace GtPrax.Application.UseCases.PatientRecord;

using FluentResults;
using Mediator;

public sealed record UpdatePatientRecordCommand(string Id, string ModifiedById, UpdatePatientRecordDto PatientRecord) : ICommand<Result>;
