namespace GtPrax.Application.UseCases.WaitingLists;

using System.Threading.Tasks;
using GtPrax.Domain.Entities;

public interface IWaitingListStore
{
    Task Create(WaitingList entity, CancellationToken cancellationToken);
    Task<WaitingListIdentity[]> GetIdentities(CancellationToken cancellationToken);
    Task<WaitingList[]> GetAll(CancellationToken cancellationToken);
}
