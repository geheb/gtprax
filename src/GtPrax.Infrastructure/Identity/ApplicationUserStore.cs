using GtPrax.Domain.Entities;
using GtPrax.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace GtPrax.Infrastructure.Identity;

internal sealed class ApplicationUserStore :
	IUserClaimStore<ApplicationUser>,
	IUserRoleStore<ApplicationUser>,
	IUserPasswordStore<ApplicationUser>,
	IUserSecurityStampStore<ApplicationUser>,
	IUserEmailStore<ApplicationUser>,
	IUserLockoutStore<ApplicationUser>
{
	private readonly IdentityErrorDescriber _identityErrorDescriber;
	private readonly TimeProvider _timeProvider;
	private readonly IMongoCollection<ApplicationUser> _collection;

	public ApplicationUserStore(
		IdentityErrorDescriber identityErrorDescriber,
		TimeProvider timeProvider, 
		MongoConnectionFactory connectionFactory)
	{
		_identityErrorDescriber = identityErrorDescriber;
		_timeProvider = timeProvider;
		_collection = connectionFactory.GetUsersCollection();
	}

	public void Dispose()
	{
	}

	public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentNullException.ThrowIfNull(claims);

		foreach (var claim in claims.Select(c => new ApplicationUserClaim(c)))
		{
			if (!user.Claims.Contains(claim))
			{
				user.Claims.Add(claim);
			}
		}

		return Task.CompletedTask;
	}

	public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(user.UserName);

		user.Id = ObjectId.GenerateNewId();
		user.CreatedDate = _timeProvider.GetUtcNow();

		await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);

		return IdentityResult.Success;
	}

	public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);

		var filter = Builders<ApplicationUser>.Filter.Eq(f => f.Id, user.Id);
		var result = await _collection.DeleteOneAsync(filter, cancellationToken);

		if (!result.IsAcknowledged)
		{
			return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
		}

		return IdentityResult.Success;
	}

	public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(normalizedEmail);

		var filter = Builders<ApplicationUser>.Filter.Regex(f => f.Email, new(normalizedEmail, "i"));
		var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

		return await entity.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(userId);

		var filter = Builders<ApplicationUser>.Filter.Eq(f => f.Id, ObjectId.Parse(userId));
		var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

		return await entity.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(normalizedUserName);

		var filter = Builders<ApplicationUser>.Filter.Regex(f => f.UserName, new(normalizedUserName, "i"));
		var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

		return await entity.FirstOrDefaultAsync(cancellationToken);
	}

	public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.AccessFailedCount);
	}

	public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		var result = user
			.Claims
			.Select(x => x.ToClaim())
			.ToList();
		return Task.FromResult<IList<Claim>>(result);
	}

	public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.Email);
	}

	public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(user.Email);
		return Task.FromResult(user.EmailConfirmed);
	}

	public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.IsLockoutEnabled);
	}

	public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.LockoutEndDate);
	}

	public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.Email);
	}

	public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.UserName);
	}

	public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.PasswordHash);
	}

	public Task<string?> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.SecurityStamp);
	}

	public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.Id.ToString());
	}

	public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(user.UserName);
	}

	public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(claim);

		var filter = Builders<ApplicationUser>.Filter.ElemMatch(f => f.Claims,
			Builders<ApplicationUserClaim>.Filter.And(
				Builders<ApplicationUserClaim>.Filter.Eq(f => f.Type, claim.Type),
				Builders<ApplicationUserClaim>.Filter.Eq(f => f.Value, claim.Value)));

		var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

		return await documents.ToListAsync(cancellationToken);
	}

	public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
	}

	public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		user.AccessFailedCount++;
		return await Task.FromResult(user.AccessFailedCount);
	}

	public Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentNullException.ThrowIfNull(claims);
		foreach (var claim in claims.Select(claim => new ApplicationUserClaim(claim)))
		{
			user.Claims.Remove(claim);
		}
		return Task.CompletedTask;
	}

	public Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentNullException.ThrowIfNull(claim);
		ArgumentNullException.ThrowIfNull(newClaim);

		var userClaim = new ApplicationUserClaim(claim);
		user.Claims.Remove(userClaim);
		user.Claims.Add(new ApplicationUserClaim(newClaim));
		return Task.CompletedTask;
	}

	public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		user.AccessFailedCount = 0;
		return Task.CompletedTask;
	}

	public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		user.Email = email;
		return Task.CompletedTask;
	}

	public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(user.Email);
		user.EmailConfirmed = confirmed;
		return Task.CompletedTask;
	}

	public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		user.IsLockoutEnabled = enabled;
		return Task.CompletedTask;
	}

	public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		user.LockoutEndDate = lockoutEnd;
		return Task.CompletedTask;
	}

	public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(user.Email);
		return Task.CompletedTask;
	}

	public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(user.UserName);
		return Task.CompletedTask;
	}

	public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		user.PasswordHash = passwordHash;
		user.LastPasswordChangedDate = _timeProvider.GetUtcNow();
		return Task.CompletedTask;
	}

	public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(stamp);
		user.SecurityStamp = stamp;
		return Task.CompletedTask;
	}

	public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		user.UserName = userName;
		return Task.CompletedTask;
	}

	public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		user.ModifiedDate = _timeProvider.GetUtcNow();

		var filter = Builders<ApplicationUser>.Filter.Eq(f => f.Id, user.Id);
		var result = await _collection.ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);
		if (!result.IsAcknowledged)
		{
			return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
		}

		return IdentityResult.Success;
	}

	public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(roleName);
		if (await IsInRoleAsync(user, roleName, cancellationToken))
		{
			throw new InvalidOperationException("User already has a role");
		}
		var role = new ApplicationUserRole(roleName);
		user.Roles.Add(role);
	}

	public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(roleName);
		user.Roles.Remove(new ApplicationUserRole(roleName));
		return Task.CompletedTask;
	}

	public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		var roles = user.Roles.Select(r => r.Name).ToList();
		return Task.FromResult<IList<string>>(roles);
	}

	public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(user);
		ArgumentException.ThrowIfNullOrWhiteSpace(roleName);
		var hasRole = user.Roles.Contains(new ApplicationUserRole(roleName));
		return Task.FromResult(hasRole);
	}

	public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

		var filter = Builders<ApplicationUser>.Filter.ElemMatch(f => f.Roles,
				Builders<ApplicationUserRole>.Filter.Eq(f => f.Name, roleName));

		var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);

		return await documents.ToListAsync(cancellationToken);
	}
}
