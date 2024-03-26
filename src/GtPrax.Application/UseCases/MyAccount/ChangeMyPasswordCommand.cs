namespace GtPrax.Application.UseCases.MyAccount;

using FluentResults;
using Mediator;

public sealed record ChangeMyPasswordCommand(string Id, string CurrentPassword, string NewPassword) : IRequest<Result>;
