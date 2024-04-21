namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record UpdateUserCommand(string Id, string? Name, string? Email, string? Password, UserRole[] Roles) : ICommand<Result>;
