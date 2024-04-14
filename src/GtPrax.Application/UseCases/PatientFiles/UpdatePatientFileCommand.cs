namespace GtPrax.Application.UseCases.PatientFiles;

using FluentResults;
using Mediator;

public sealed record UpdatePatientFileCommand(string Id, string ModifiedBy, UpdatePatientFileDto PatientFile) : ICommand<Result>;
