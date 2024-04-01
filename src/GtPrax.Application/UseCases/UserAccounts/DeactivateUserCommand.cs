namespace GtPrax.Application.UseCases.UserAccounts;

using FluentResults;
using Mediator;

public sealed record DeactivateUserCommand(string Id) : ICommand<Result>;
