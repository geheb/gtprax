namespace GtPrax.Application.UseCases.UserAccounts;

using FluentResults;
using Mediator;

public sealed record ConfirmChangeMyEmailCommand(string Id, string Token, string NewEmail) : ICommand<Result>;
