namespace GtPrax.Application.UseCases.UserAccounts;

using FluentResults;
using Mediator;

public sealed record UpdateMyUserCommand(string Id, string Name) : ICommand<Result>;
