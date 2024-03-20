namespace GtPrax.Application.UseCases.MyAccount;

using FluentResults;
using Mediator;

public sealed record UpdateMyUserCommand(string UserId, string Name) : IRequest<Result>;
