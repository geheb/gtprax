namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record ConfirmResetPasswordCommand(string Id, string Token, string NewPassword) : ICommand<Result>;
