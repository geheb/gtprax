namespace GtPrax.Application.UseCases.UserAccount;

using FluentResults;
using Mediator;

public sealed record ChangeMyPasswordCommand(string Id, string CurrentPassword, string NewPassword) : ICommand<Result>;
