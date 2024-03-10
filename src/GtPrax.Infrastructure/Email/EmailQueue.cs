namespace GtPrax.Infrastructure.Email;

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

internal sealed class EmailQueue
{
    public ObjectId Id { get; set; }

    public required string Email { get; set; }

    public required string Subject { get; set; }

    public required string Message { get; set; }

    [BsonRepresentation(BsonType.String)]
    public required DateTimeOffset CreatedOn { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? SentOn { get; set; }
}
