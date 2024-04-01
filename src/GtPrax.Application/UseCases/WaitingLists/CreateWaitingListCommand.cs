namespace GtPrax.Application.UseCases.WaitingLists;

using FluentResults;
using Mediator;

public sealed record CreateWaitingListCommand(string Name) : ICommand<Result>;
