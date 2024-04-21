namespace GtPrax.Application.UseCases.PatientRecord;

using FluentResults;
using Mediator;

public sealed record CreatePatientRecordCommand(string WaitingListId, string CreatedBy, CreatePatientRecordDto PatientRecord) : ICommand<Result>;
