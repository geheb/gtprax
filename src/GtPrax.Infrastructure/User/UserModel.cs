namespace GtPrax.Infrastructure.User;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

internal sealed class UserModel
{
    public required ObjectId Id { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public bool IsLockoutEnabled { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public int AccessFailedCount { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? CreatedDate { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? ModifiedDate { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? LockoutEndDate { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? LastPasswordChangedDate { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? LastLoginDate { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset? DeactivationDate { get; set; }

    public required string Name { get; set; }
    public string? AuthenticatorKey { get; set; }

    public ICollection<UserClaimModel> Claims { get; set; } = new List<UserClaimModel>();
}
