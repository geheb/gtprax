using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GtPrax.Infrastructure.Identity;

internal sealed class ApplicationUser
{
	public ObjectId Id { get; set; }

	public string? UserName { get; set; }

	public string? Email { get; set; }

	public bool EmailConfirmed { get; set; }

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

	public bool IsPasswordReset { get; set; }

	public string? Name { get; set; }

	public ICollection<ApplicationUserClaim> Claims { get; set; } = new List<ApplicationUserClaim>();

	public ICollection<ApplicationUserRole> Roles { get; set; } = new List<ApplicationUserRole>();
}
