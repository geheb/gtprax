namespace GtPrax.Application.UseCases.PatientFiles;

using FluentResults;
using Mediator;

public sealed record CreatePatientRecordCommand(string WaitingListId, string CreatedBy, CreatePatientRecordDto PatientRecord) : ICommand<Result>;
