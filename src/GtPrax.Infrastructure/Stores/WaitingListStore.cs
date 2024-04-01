namespace GtPrax.Infrastructure.Stores;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.UseCases.WaitingLists;
using GtPrax.Domain.Entities;
using GtPrax.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

internal sealed class WaitingListStore : IWaitingListStore
{
    private readonly IMongoCollection<WaitingListModel> _collection;

    public WaitingListStore(
        MongoConnectionFactory connectionFactory)
    {
        _collection = connectionFactory.GetWaitingListsCollection();
    }


    public async Task Create(WaitingList entity, CancellationToken cancellationToken)
    {
        var model = entity.MapToModel(ObjectId.GenerateNewId());
        await _collection.InsertOneAsync(model, cancellationToken: cancellationToken);
    }

    public async Task<WaitingListIdentity[]> GetIdentities(CancellationToken cancellationToken)
    {
        var filter = Builders<WaitingListModel>.Filter.Empty;
        var find = new FindOptions<WaitingListModel>
        {
            Projection = Builders<WaitingListModel>.Projection.Include(f => f.Id).Include(f => f.Name)
        };

        var result = await _collection.FindAsync(filter, find, cancellationToken);
        var models = await result.ToListAsync(cancellationToken);
        return models.MapToIdentityDomain();
    }

    public async Task<WaitingList[]> GetAll(CancellationToken cancellationToken)
    {
        var filter = Builders<WaitingListModel>.Filter.Empty;

        var result = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        var models = await result.ToListAsync(cancellationToken);
        return models.MapToDomain();
    }
}
