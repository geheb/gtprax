namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record SignInCommand(string Email, string Password) : IRequest<Result>;
