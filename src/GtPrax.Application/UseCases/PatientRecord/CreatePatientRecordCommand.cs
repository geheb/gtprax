namespace GtPrax.Application.UseCases.PatientRecord;

using FluentResults;
using Mediator;

public sealed record CreatePatientRecordCommand(string WaitingListItemId, string CreatedBy, CreatePatientRecordDto PatientRecord) : ICommand<Result>;
