namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record VerifyResetPasswordTokenQuery(string Id, string Token) : IQuery<Result>;
