namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record ConfirmResetPasswordCommand(string UserId, string Token, string NewPassword) : IRequest<Result>;
