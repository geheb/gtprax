namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record CreateUserTwoFactorCommand(string Id, string AppName) : ICommand<Result<UserTwoFactorDto>>;
