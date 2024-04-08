namespace GtPrax.Infrastructure.Stores;

using System.Threading;
using System.Threading.Tasks;
using GtPrax.Application.UseCases.PatientFiles;
using GtPrax.Domain.Entities;
using GtPrax.Infrastructure.Mongo;
using MongoDB.Driver;

internal sealed class PatientFileStore : IPatientFileStore
{
    private readonly IMongoCollection<PatientFileModel> _collection;

    public PatientFileStore(
        MongoConnectionFactory connectionFactory)
    {
        _collection = connectionFactory.GetPatientFilesCollection();
    }

    public async Task Create(PatientFile entity, CancellationToken cancellationToken)
    {
        var model = entity.MapToModel();
        await _collection.InsertOneAsync(model, cancellationToken: cancellationToken);
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
        return models.MapToDomain();
    }
}
