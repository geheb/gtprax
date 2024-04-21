namespace GtPrax.Application.UseCases.WaitingList;

using Mediator;

public sealed record FindWaitingListByIdQuery(string Id) : IQuery<WaitingListDto?>;
