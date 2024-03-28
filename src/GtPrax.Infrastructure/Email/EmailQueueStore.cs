namespace GtPrax.Infrastructure.Email;

using System.Threading;
using GtPrax.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

internal sealed class EmailQueueStore
{
    private readonly IMongoCollection<EmailQueueModel> _collection;
    private readonly TimeProvider _timeProvider;

    public EmailQueueStore(
        TimeProvider timeProvider,
        MongoConnectionFactory connectionFactory)
    {
        _collection = connectionFactory.GetEmailQueueCollection();
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<EmailQueueModel>> GetNotSentLimited(CancellationToken cancellationToken)
    {
        var filter = Builders<EmailQueueModel>.Filter.Eq(f => f.SentOn, null);
        var options = new FindOptions<EmailQueueModel>
        {
            Limit = 100
        };

        using var documents = await _collection.FindAsync(filter, options, cancellationToken);
        return await documents.ToListAsync(cancellationToken);
    }

    public Task Add(EmailQueueModel entity, CancellationToken cancellationToken)
        => _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public async Task<bool> UpdateSentOn(ObjectId id, CancellationToken cancellationToken)
    {
        var filter = Builders<EmailQueueModel>.Filter.Eq(f => f.Id, id);
        var update = Builders<EmailQueueModel>.Update.Set(f => f.SentOn, _timeProvider.GetUtcNow());

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.IsAcknowledged;
    }
}
