namespace GtPrax.Infrastructure.Repositories;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Domain.Models;
using GtPrax.Domain.Repositories;
using GtPrax.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

internal sealed class PatientRecordRepo : IPatientRecordRepo
{
    private readonly IMongoCollection<PatientRecordModel> _collection;

    public PatientRecordRepo(
        MongoConnectionFactory connectionFactory)
    {
        _collection = connectionFactory.GetPatientRecordCollection();
    }

    public async Task Upsert(PatientRecord entity, CancellationToken cancellationToken)
    {
        var model = entity.MapToModel();

        var filter = Builders<PatientRecordModel>.Filter.Eq(f => f.Id, model.Id);
        var option = new ReplaceOptions
        {
            IsUpsert = true
        };
        await _collection.ReplaceOneAsync(filter, model, option, cancellationToken);
    }

    public async Task<PatientRecord?> Find(PatientRecordId id, CancellationToken cancellationToken)
    {
        var filter = Builders<PatientRecordModel>.Filter.Eq(f => f.Id, ObjectId.Parse(id));
        using var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return (await cursor.FirstOrDefaultAsync(cancellationToken))?.MapToDomain();
    }

    public async Task<PatientRecord[]> GetAll(CancellationToken cancellationToken)
    {
        var filter = Builders<PatientRecordModel>.Filter.Empty;

        using var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        var models = await cursor.ToListAsync(cancellationToken);
        return models.MapToDomain();
    }
}
