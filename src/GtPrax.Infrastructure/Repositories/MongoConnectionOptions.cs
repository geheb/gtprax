namespace GtPrax.Infrastructure.Repositories;

public sealed class MongoConnectionOptions
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string DatabaseName { get; set; }
    public required string UsersCollectionName { get; set; }
}
