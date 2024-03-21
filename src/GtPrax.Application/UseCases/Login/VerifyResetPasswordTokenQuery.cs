namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record VerifyResetPasswordTokenQuery(string UserId, string Token) : IQuery<Result>;
