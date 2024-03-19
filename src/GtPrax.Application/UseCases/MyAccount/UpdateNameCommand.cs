namespace GtPrax.Application.UseCases.MyAccount;

using FluentResults;
using Mediator;

public sealed record UpdateNameCommand(string Id, string Name) : IRequest<Result>;
