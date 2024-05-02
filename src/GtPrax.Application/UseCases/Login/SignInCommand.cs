namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using GtPrax.Application.Services;
using Mediator;

public sealed record SignInCommand(string Email, string Password) : ICommand<Result<SignInAction>>;
