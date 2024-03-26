namespace GtPrax.Application.UseCases.Login;

using FluentResults;
using Mediator;

public sealed record VerifyConfirmEmailTokenQuery(string Id, string Token) : IQuery<Result>;
