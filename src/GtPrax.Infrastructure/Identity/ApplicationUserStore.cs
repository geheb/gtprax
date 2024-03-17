namespace GtPrax.Infrastructure.Identity;

using System.Security.Claims;
using GtPrax.Infrastructure.Mongo;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

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

    public async Task<IReadOnlyCollection<ApplicationUser>> GetAllUsers(CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(f => f.DeactivationDate, null);
        var result = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IdentityResult> SetName(string id, string name, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(f => f.Id, ObjectId.Parse(id));
        var update = Builders<ApplicationUser>.Update.Set(f => f.Name, name);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        if (!result.IsAcknowledged || result.ModifiedCount < 1)
        {
            return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
        }
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> SetLastLogin(string id, DateTimeOffset lastLogin, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(f => f.Id, ObjectId.Parse(id));
        var update = Builders<ApplicationUser>.Update.Set(f => f.LastLoginDate, lastLogin);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        if (!result.IsAcknowledged || result.ModifiedCount < 1)
        {
            return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
        }
        return IdentityResult.Success;
    }

    public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
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
        user.Id = ObjectId.GenerateNewId();
        user.CreatedDate = _timeProvider.GetUtcNow();
        await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
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
        var filter = Builders<ApplicationUser>.Filter.Regex(f => f.Email, new(normalizedEmail, "i"));
        using var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await entity.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(f => f.Id, ObjectId.Parse(userId));
        using var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await entity.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Regex(f => f.UserName, new(normalizedUserName, "i"));
        using var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await entity.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.AccessFailedCount);

    public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var result = user.Claims.Select(x => x.ToClaim()).ToList();
        return Task.FromResult<IList<Claim>>(result);
    }

    public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.Email);

    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.EmailConfirmed);

    public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.IsLockoutEnabled);

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.LockoutEndDate);

    public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.Email);

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.UserName);

    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.PasswordHash);

    public Task<string?> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.SecurityStamp);

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.Id.ToString());

    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(user.UserName);

    public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.ElemMatch(f => f.Claims,
            Builders<ApplicationUserClaim>.Filter.And(
                Builders<ApplicationUserClaim>.Filter.Eq(f => f.Type, claim.Type),
                Builders<ApplicationUserClaim>.Filter.Eq(f => f.Value, claim.Value)));

        using var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await documents.ToListAsync(cancellationToken);
    }

    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

    public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        => Task.FromResult(++user.AccessFailedCount);

    public Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims.Select(claim => new ApplicationUserClaim(claim)))
        {
            user.Claims.Remove(claim);
        }
        return Task.CompletedTask;
    }

    public Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        var userClaim = new ApplicationUserClaim(claim);
        user.Claims.Remove(userClaim);
        user.Claims.Add(new ApplicationUserClaim(newClaim));
        return Task.CompletedTask;
    }

    public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.IsLockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        user.LockoutEndDate = lockoutEnd;
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        user.LastPasswordChangedDate = _timeProvider.GetUtcNow();
        return Task.CompletedTask;
    }

    public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        user.ModifiedDate = _timeProvider.GetUtcNow();

        var filter = Builders<ApplicationUser>.Filter.Eq(f => f.Id, user.Id);
        var result = await _collection.ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);
        if (!result.IsAcknowledged || result.ModifiedCount < 1)
        {
            return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
        }
        return IdentityResult.Success;
    }

    public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        var role = new ApplicationUserRole(roleName);
        if (!user.Roles.Contains(role))
        {
            user.Roles.Add(role);
        }
        return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        user.Roles.Remove(new ApplicationUserRole(roleName));
        return Task.CompletedTask;
    }

    public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = user.Roles.Select(r => r.Name).ToList();
        return Task.FromResult<IList<string>>(roles);
    }

    public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        var hasRole = user.Roles.Contains(new ApplicationUserRole(roleName));
        return Task.FromResult(hasRole);
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.ElemMatch(f => f.Roles,
                Builders<ApplicationUserRole>.Filter.Eq(f => f.Name, roleName));

        using var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await documents.ToListAsync(cancellationToken);
    }
}
