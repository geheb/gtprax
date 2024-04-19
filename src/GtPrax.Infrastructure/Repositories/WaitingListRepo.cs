namespace GtPrax.Infrastructure.Repositories;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Domain.Models;
using GtPrax.Domain.Repositories;
using GtPrax.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

internal sealed class WaitingListRepo : IWaitingListRepo
{
    private readonly IMongoCollection<WaitingListModel> _collection;

    public WaitingListRepo(
        MongoConnectionFactory connectionFactory)
    {
        _collection = connectionFactory.GetWaitingListsCollection();
    }

    public async Task Upsert(WaitingListItem entity, CancellationToken cancellationToken)
    {
        var model = entity.MapToModel();

        var filter = Builders<WaitingListModel>.Filter.Eq(f => f.Id, model.Id);
        var option = new ReplaceOptions
        {
            IsUpsert = true
        };
        await _collection.ReplaceOneAsync(filter, model, option, cancellationToken);
    }

    public async Task<WaitingListItem?> Find(WaitingListItemId id, CancellationToken cancellationToken)
    {
        var filter = Builders<WaitingListModel>.Filter.Eq(f => f.Id, ObjectId.Parse(id.Value));
        using var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return (await cursor.FirstOrDefaultAsync(cancellationToken))?.MapToDomain();
    }

    public async Task<WaitingListItem[]> GetAll(CancellationToken cancellationToken)
    {
        var filter = Builders<WaitingListModel>.Filter.Empty;

        using var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        var models = await cursor.ToListAsync(cancellationToken);
        return models.MapToDomain();
    }
}
