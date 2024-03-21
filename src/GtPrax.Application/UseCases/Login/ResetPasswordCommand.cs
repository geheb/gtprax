namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record ResetPasswordCommand(string Email, string CallbackUrl) : IRequest<Result>;
