namespace GtPrax.Application.UseCases.UsersManagement;

using FluentResults;
using Mediator;

public sealed record SendConfirmEmailCommand(string Id, string CallbackUrl) : IRequest<Result>;
