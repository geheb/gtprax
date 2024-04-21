namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record SendConfirmEmailCommand(string Id, string CallbackUrl) : ICommand<Result>;
