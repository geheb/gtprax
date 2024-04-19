namespace GtPrax.Domain.Repositories;

using System.Threading.Tasks;
using GtPrax.Domain.Models;

public interface IWaitingListRepo
{
    Task Upsert(WaitingListItem entity, CancellationToken cancellationToken);
    Task<WaitingListItem?> Find(WaitingListItemId id, CancellationToken cancellationToken);
    Task<WaitingListItem[]> GetAll(CancellationToken cancellationToken);
}
