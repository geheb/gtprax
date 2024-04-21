namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record UpdateMyUserCommand(string Id, string Name) : ICommand<Result>;
