namespace GtPrax.Application.UseCases.UserAccounts;

using FluentResults;
using Mediator;

public sealed record SendConfirmEmailCommand(string Id, string CallbackUrl) : ICommand<Result>;
