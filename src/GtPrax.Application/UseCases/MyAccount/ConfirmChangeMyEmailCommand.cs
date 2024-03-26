namespace GtPrax.Application.UseCases.MyAccount;

using FluentResults;
using Mediator;

public sealed record ConfirmChangeMyEmailCommand(string Id, string Token, string NewEmail) : IRequest<Result>;
