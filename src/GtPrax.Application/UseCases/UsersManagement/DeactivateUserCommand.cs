namespace GtPrax.Application.UseCases.UsersManagement;

using FluentResults;
using Mediator;

public sealed record DeactivateUserCommand(string Id) : IRequest<Result>;
