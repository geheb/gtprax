namespace GtPrax.Infrastructure.Stores;

using MongoDB.Bson;

internal sealed class WaitingListModel
{
    public required ObjectId Id { get; set; }
    public required string Name { get; set; }
}
