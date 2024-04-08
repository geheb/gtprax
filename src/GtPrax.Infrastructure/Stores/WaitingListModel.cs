namespace GtPrax.Infrastructure.Stores;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

internal sealed class WaitingListModel
{
    public required ObjectId Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public required DateTimeOffset CreatedDate { get; set; }

    public required string CreatedBy { get; set; }

    public required string Name { get; set; }
}
