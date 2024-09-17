namespace GtPrax.Infrastructure.Mongo;

internal sealed class MongoConnectionOptions
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string DatabaseName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string UserCollectionName { get; set; }
    public required string EmailQueueCollectionName { get; set; }
    public required string WaitingListCollectionName { get; set; }
    public required string PatientRecordCollectionName { get; set; }
}
