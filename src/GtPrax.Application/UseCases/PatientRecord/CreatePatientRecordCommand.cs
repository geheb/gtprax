namespace GtPrax.Application.UseCases.PatientRecord;

using FluentResults;
using Mediator;

public sealed record CreatePatientRecordCommand(string WaitingListItemId, string CreatedById, CreatePatientRecordDto PatientRecord) : ICommand<Result>;
