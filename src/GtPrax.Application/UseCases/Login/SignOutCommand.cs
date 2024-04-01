namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record SignOutCommand() : ICommand<Result>;
