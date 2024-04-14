namespace GtPrax.Application.UseCases.PatientFiles;

using FluentResults;
using Mediator;

public sealed record CreatePatientFileCommand(string WaitingListId, string CreatedBy, CreatePatientFileDto PatientFile) : ICommand<Result>;
