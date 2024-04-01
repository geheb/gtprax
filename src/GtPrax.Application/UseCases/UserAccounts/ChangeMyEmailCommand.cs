namespace GtPrax.Application.UseCases.UserAccounts;

using FluentResults;
using Mediator;

public sealed record ChangeMyEmailCommand(string Id, string CurrentPassword, string NewEmail, string CallbackUrl) : ICommand<Result>;
