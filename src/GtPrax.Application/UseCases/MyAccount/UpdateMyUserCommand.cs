namespace GtPrax.Application.UseCases.MyAccount;

using FluentResults;
using Mediator;

public sealed record UpdateMyUserCommand(string Id, string Name) : IRequest<Result>;
