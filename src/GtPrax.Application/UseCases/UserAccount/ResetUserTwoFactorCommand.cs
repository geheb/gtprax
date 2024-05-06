namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record ResetUserTwoFactorCommand(string Id) : ICommand<Result>;
