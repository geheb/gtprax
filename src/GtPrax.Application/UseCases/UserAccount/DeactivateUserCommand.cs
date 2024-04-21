namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record DeactivateUserCommand(string Id) : ICommand<Result>;
