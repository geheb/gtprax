namespace GtPrax.Application.UseCases.UsersManagement;

using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

public sealed record UpdateUserCommand(string Id, string? Name, string? Email, string? Password, UserRole[] Roles) : IRequest<Result>;
