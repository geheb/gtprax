namespace GtPrax.Application.UseCases.MyAccount;

using FluentResults;
using Mediator;

public sealed record ConfirmChangeMyEmailCommand(string UserId, string Token, string NewEmail) : IRequest<Result>;
