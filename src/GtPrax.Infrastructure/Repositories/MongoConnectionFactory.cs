namespace GtPrax.Infrastructure.Repositories;

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

    public IMongoCollection<ApplicationUser> GetUsersCollection()
    {
        var client = new MongoClient(_settings);
        var database = client.GetDatabase(_options.DatabaseName);
        return database.GetCollection<ApplicationUser>(_options.UsersCollectionName);
    }
}
