namespace GtPrax.Application.UseCases.WaitingList;

using Mediator;

public sealed record GetWaitingListIndexQuery() : IQuery<WaitingListIndexDto[]>;
