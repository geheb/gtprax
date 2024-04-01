namespace GtPrax.Application.UseCases.UserAccounts;

using FluentResults;
using Mediator;

public sealed record CreateUserCommand(string Name, string Email, UserRole[] Roles, string CallbackUrl) : ICommand<Result>;

