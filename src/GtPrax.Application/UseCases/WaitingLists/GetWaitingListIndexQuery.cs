namespace GtPrax.Application.UseCases.WaitingLists;

using Mediator;

public sealed record GetWaitingListIndexQuery() : IQuery<WaitingListIndexDto[]>;
