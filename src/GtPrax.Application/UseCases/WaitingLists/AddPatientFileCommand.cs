namespace GtPrax.Application.UseCases.WaitingLists;

using FluentResults;
using Mediator;

public sealed record AddPatientFileCommand(string WaitingListId, PatientFileDto PatientFile) : ICommand<Result>;
