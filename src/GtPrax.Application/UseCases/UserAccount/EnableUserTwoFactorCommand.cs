namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record EnableUserTwoFactorCommand(string Id, bool Enable, string Code) : ICommand<Result>;
