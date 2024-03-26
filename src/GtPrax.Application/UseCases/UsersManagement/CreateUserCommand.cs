namespace GtPrax.Application.UseCases.UsersManagement;

using FluentResults;
using GtPrax.Application.Identity;
using Mediator;

public sealed record CreateUserCommand(string Name, string Email, UserRole[] Roles, string CallbackUrl) : IRequest<Result>;

