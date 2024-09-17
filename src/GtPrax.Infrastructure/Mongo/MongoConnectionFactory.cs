namespace GtPrax.Infrastructure.Mongo;

using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Repositories;
using GtPrax.Infrastructure.User;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

internal sealed class MongoConnectionFactory
{
    private readonly MongoConnectionOptions _options;
    private readonly MongoClient _client;

    public MongoConnectionFactory(IOptions<MongoConnectionOptions> options)
    {
        _options = options.Value;

        var identity = new MongoInternalIdentity("admin", _options.Username);
        var evidence = new PasswordEvidence(_options.Password);

        var settings = new MongoClientSettings()
        {
            Scheme = ConnectionStringScheme.MongoDB,
            Server = new MongoServerAddress(_options.Host, _options.Port),
            ConnectTimeout = TimeSpan.FromSeconds(30),
            UseTls = false,
            ApplicationName = "GtPrax",
            Credential = new MongoCredential("SCRAM-SHA-256", identity, evidence)
        };
        _client = new MongoClient(settings);
    }

    public IMongoCollection<UserModel> GetUsersCollection()
    {
        var database = _client.GetDatabase(_options.DatabaseName);
        return database.GetCollection<UserModel>(_options.UserCollectionName);
    }

    public IMongoCollection<EmailQueueModel> GetEmailQueueCollection()
    {
        var database = _client.GetDatabase(_options.DatabaseName);
        return database.GetCollection<EmailQueueModel>(_options.EmailQueueCollectionName);
    }

    public IMongoCollection<WaitingListModel> GetWaitingListsCollection()
    {
        var database = _client.GetDatabase(_options.DatabaseName);
        return database.GetCollection<WaitingListModel>(_options.WaitingListCollectionName);
    }

    public IMongoCollection<PatientRecordModel> GetPatientRecordCollection()
    {
        var database = _client.GetDatabase(_options.DatabaseName);
        return database.GetCollection<PatientRecordModel>(_options.PatientRecordCollectionName);
    }
}
