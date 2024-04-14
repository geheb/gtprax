namespace GtPrax.Infrastructure.Stores;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

internal sealed class PatientFileModel
{
    public required ObjectId Id { get; set; }

    public required ObjectId WaitingListId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public required DateTimeOffset CreatedDate { get; set; }

    public required ObjectId CreatedById { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? LastModifiedDate { get; set; }

    public ObjectId? LastModifiedById { get; set; }

    public required string Name { get; set; }

    [BsonDateTimeOptions(DateOnly = true, Representation = BsonType.String)]
    public required DateTime BirthDate { get; set; }

    public required string PhoneNumber { get; set; }

    public string? ReferralReason { get; set; }

    public string? ReferralDoctor { get; set; }

    public TherapyDayModel[]? TherapyDays { get; set; }

    public int[]? Tags { get; set; }

    public string? Remark { get; set; }
}
