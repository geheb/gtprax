namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record ConfirmEmailCommand(string Id, string Token, string Password) : ICommand<Result>;
