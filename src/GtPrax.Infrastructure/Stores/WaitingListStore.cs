namespace GtPrax.Infrastructure.Stores;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.UseCases.WaitingLists;
using GtPrax.Domain.Entities;
using GtPrax.Infrastructure.Mongo;
using MongoDB.Driver;

internal sealed class WaitingListStore : IWaitingListStore
{
    private readonly IMongoCollection<WaitingListModel> _collection;

    public WaitingListStore(
        MongoConnectionFactory connectionFactory)
    {
        _collection = connectionFactory.GetWaitingListsCollection();
    }

    public async Task Upsert(WaitingList entity, CancellationToken cancellationToken)
    {
        var model = entity.MapToModel();
        if (string.IsNullOrEmpty(entity.Identity.Id))
        {
            await _collection.InsertOneAsync(model, cancellationToken: cancellationToken);
        }
        else
        {
            var filter = Builders<WaitingListModel>.Filter.Eq(f => f.Id, model.Id);
            await _collection.ReplaceOneAsync(filter, model, cancellationToken: cancellationToken);
        }
    }

    public async Task<WaitingListIdentity[]> GetIdentities(CancellationToken cancellationToken)
    {
        var filter = Builders<WaitingListModel>.Filter.Empty;
        var find = new FindOptions<WaitingListModel>
        {
            Projection = Builders<WaitingListModel>.Projection.Include(f => f.Id).Include(f => f.Name)
        };

        var cursor = await _collection.FindAsync(filter, find, cancellationToken);
        var models = await cursor.ToListAsync(cancellationToken);
        return models.MapToIdentityDomain();
    }

    public async Task<WaitingList[]> GetAll(CancellationToken cancellationToken)
    {
        var filter = Builders<WaitingListModel>.Filter.Empty;

        var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        var models = await cursor.ToListAsync(cancellationToken);
        return models.MapToDomain();
    }
}
