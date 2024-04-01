namespace GtPrax.Infrastructure.Mongo;

internal sealed class MongoConnectionOptions
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string DatabaseName { get; set; }
    public required string UsersCollectionName { get; set; }
    public required string EmailQueueCollectionName { get; set; }
    public required string WaitingListsCollectionName { get; set; }
}
