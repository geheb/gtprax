namespace GtPrax.Application.UseCases.WaitingLists;

using System.Threading;
using System.Threading.Tasks;
using Mediator;

internal sealed class GetWaitingListIndexHandler : IQueryHandler<GetWaitingListIndexQuery, WaitingListIndexDto[]>
{
    private readonly IWaitingListStore _store;

    public GetWaitingListIndexHandler(IWaitingListStore store)
    {
        _store = store;
    }

    public async ValueTask<WaitingListIndexDto[]> Handle(GetWaitingListIndexQuery query, CancellationToken cancellationToken)
    {
        var waitingLists = await _store.GetAll(cancellationToken);
        return waitingLists.Select(w => new WaitingListIndexDto(w.Identity.Id!, w.Identity.Name, 0)).ToArray();
    }
}
