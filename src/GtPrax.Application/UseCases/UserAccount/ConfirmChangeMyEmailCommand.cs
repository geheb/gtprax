namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record ConfirmChangeMyEmailCommand(string Id, string Token, string NewEmail) : ICommand<Result>;
