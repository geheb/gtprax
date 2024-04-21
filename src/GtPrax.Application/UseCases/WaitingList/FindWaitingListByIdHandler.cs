namespace GtPrax.Application.UseCases.WaitingList;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Domain.Repositories;
using Mediator;

internal sealed class FindWaitingListByIdHandler : IQueryHandler<FindWaitingListByIdQuery, WaitingListDto?>
{
    private readonly IWaitingListRepo _waitingListRepo;

    public FindWaitingListByIdHandler(IWaitingListRepo waitingListRepo)
    {
        _waitingListRepo = waitingListRepo;
    }

    public async ValueTask<WaitingListDto?> Handle(FindWaitingListByIdQuery query, CancellationToken cancellationToken) =>
        (await _waitingListRepo.Find(query.Id, cancellationToken))?.MapToDto();

}
