namespace GtPrax.Application.UseCases.MyAccount;

using FluentResults;
using Mediator;

public sealed record ChangeMyEmailCommand(string Id, string CurrentPassword, string NewEmail, string CallbackUrl) : IRequest<Result>;
