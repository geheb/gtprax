namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record SignInTwoFactorCommand(string Code, bool RememberClient) : ICommand<Result>;
