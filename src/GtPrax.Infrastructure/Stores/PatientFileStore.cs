namespace GtPrax.Infrastructure.Stores;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.UseCases.PatientFiles;
using GtPrax.Domain.Entities;
using GtPrax.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

internal sealed class PatientFileStore : IPatientFileStore
{
    private readonly IMongoCollection<PatientFileModel> _collection;
    private readonly TimeProvider _timeProvider;

    public PatientFileStore(
        TimeProvider timeProvider,
        MongoConnectionFactory connectionFactory)
    {
        _collection = connectionFactory.GetPatientFilesCollection();
        _timeProvider = timeProvider;
    }

    public async Task Upsert(PatientFile entity, CancellationToken cancellationToken)
    {
        var model = entity.MapToModel();
        if (string.IsNullOrEmpty(entity.Id))
        {
            await _collection.InsertOneAsync(model, cancellationToken: cancellationToken);
        }
        else
        {
            var filter = Builders<PatientFileModel>.Filter.Eq(f => f.Id, model.Id);
            await _collection.ReplaceOneAsync(filter, model, cancellationToken: cancellationToken);
        }
    }

    public async Task<PersonIdentity[]> GetIdentities(CancellationToken cancellationToken)
    {
        var filter = Builders<PatientFileModel>.Filter.Empty;
        var find = new FindOptions<PatientFileModel>
        {
            Projection = Builders<PatientFileModel>.Projection.Include(f => f.Name).Include(f => f.BirthDate)
        };

        var cursor = await _collection.FindAsync(filter, find, cancellationToken);
        var models = await cursor.ToListAsync(cancellationToken);
        return models.MapToIdentityDomain();
    }

    public async Task<PatientFile[]> GetAll(CancellationToken cancellationToken)
    {
        var filter = Builders<PatientFileModel>.Filter.Empty;

        var cursor = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        var models = await cursor.ToListAsync(cancellationToken);
        return models.MapToDomain(_timeProvider.GetUtcNow());
    }

    public async Task<PatientFile?> Find(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<PatientFileModel>.Filter.Eq(f => f.Id, ObjectId.Parse(id));
        using var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return (await entity.FirstOrDefaultAsync(cancellationToken))?.MapToDomain(_timeProvider.GetUtcNow());
    }
}
