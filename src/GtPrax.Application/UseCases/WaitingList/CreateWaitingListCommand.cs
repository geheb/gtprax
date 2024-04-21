namespace GtPrax.Application.UseCases.WaitingList;

using FluentResults;
using Mediator;

public sealed record CreateWaitingListCommand(string Name, string CreatedById) : ICommand<Result>;
