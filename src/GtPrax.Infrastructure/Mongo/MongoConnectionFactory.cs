namespace GtPrax.Infrastructure.Mongo;

using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

internal sealed class MongoConnectionFactory
{
    private readonly MongoClientSettings _settings;
    private readonly MongoConnectionOptions _options;

    public MongoConnectionFactory(IOptions<MongoConnectionOptions> options)
    {
        _options = options.Value;
        _settings = new MongoClientSettings()
        {
            Scheme = ConnectionStringScheme.MongoDB,
            Server = new MongoServerAddress(_options.Host, _options.Port),
            ConnectTimeout = TimeSpan.FromSeconds(30),
            UseTls = false
        };
    }

    public IMongoCollection<UserModel> GetUsersCollection()
    {
        var client = new MongoClient(_settings);
        var database = client.GetDatabase(_options.DatabaseName);
        return database.GetCollection<UserModel>(_options.UsersCollectionName);
    }

    public IMongoCollection<EmailQueueModel> GetEmailQueueCollection()
    {
        var client = new MongoClient(_settings);
        var database = client.GetDatabase(_options.DatabaseName);
        return database.GetCollection<EmailQueueModel>(_options.EmailQueueCollectionName);
    }
}
