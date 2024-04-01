namespace GtPrax.Infrastructure.User;

using System.Security.Claims;
using GtPrax.Infrastructure.Mongo;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

internal sealed class UserStore :
    IUserClaimStore<UserModel>,
    IUserRoleStore<UserModel>,
    IUserPasswordStore<UserModel>,
    IUserSecurityStampStore<UserModel>,
    IUserEmailStore<UserModel>,
    IUserLockoutStore<UserModel>
{
    private readonly IdentityErrorDescriber _identityErrorDescriber;
    private readonly TimeProvider _timeProvider;
    private readonly IMongoCollection<UserModel> _collection;

    public UserStore(
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

    public async Task<IReadOnlyCollection<UserModel>> GetAllUsers(CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.Eq(f => f.DeactivationDate, null);
        var result = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IdentityResult> SetName(string id, string name, CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.Eq(f => f.Id, ObjectId.Parse(id));
        var update = Builders<UserModel>.Update.Set(f => f.Name, name);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        if (result.IsAcknowledged && (result.ModifiedCount > 0 || result.MatchedCount > 0))
        {
            return IdentityResult.Success;
        }
        return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
    }

    public async Task<IdentityResult> SetLastLogin(string id, DateTimeOffset lastLogin, CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.Eq(f => f.Id, ObjectId.Parse(id));
        var update = Builders<UserModel>.Update.Set(f => f.LastLoginDate, lastLogin);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        if (result.IsAcknowledged && result.ModifiedCount > 0)
        {
            return IdentityResult.Success;
        }
        return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
    }

    public Task AddClaimsAsync(UserModel user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims.Select(c => new UserClaimModel(c)))
        {
            if (!user.Claims.Contains(claim))
            {
                user.Claims.Add(claim);
            }
        }
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> CreateAsync(UserModel user, CancellationToken cancellationToken)
    {
        user.Id = ObjectId.GenerateNewId();
        user.CreatedDate = _timeProvider.GetUtcNow();
        await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(UserModel user, CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.Eq(f => f.Id, user.Id);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        if (result.IsAcknowledged)
        {
            return IdentityResult.Success;
        }
        return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
    }

    public async Task<UserModel?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.Regex(f => f.Email, new(normalizedEmail, "i"));
        using var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await entity.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserModel?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.Eq(f => f.Id, ObjectId.Parse(userId));
        using var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await entity.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserModel?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.Regex(f => f.UserName, new(normalizedUserName, "i"));
        using var entity = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await entity.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> GetAccessFailedCountAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.AccessFailedCount);

    public Task<IList<Claim>> GetClaimsAsync(UserModel user, CancellationToken cancellationToken)
    {
        var result = user.Claims.Select(x => x.ToClaim()).ToList();
        return Task.FromResult<IList<Claim>>(result);
    }

    public Task<string?> GetEmailAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.Email ?? null);

    public Task<bool> GetEmailConfirmedAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.IsEmailConfirmed);

    public Task<bool> GetLockoutEnabledAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.IsLockoutEnabled);

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.LockoutEndDate);

    public Task<string?> GetNormalizedEmailAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.Email ?? null);

    public Task<string?> GetNormalizedUserNameAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.UserName ?? null);

    public Task<string?> GetPasswordHashAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.PasswordHash);

    public Task<string?> GetSecurityStampAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.SecurityStamp);

    public Task<string> GetUserIdAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.Id.ToString());

    public Task<string?> GetUserNameAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(user.UserName ?? null);

    public async Task<IList<UserModel>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.ElemMatch(f => f.Claims,
            Builders<UserClaimModel>.Filter.And(
                Builders<UserClaimModel>.Filter.Eq(f => f.Type, claim.Type),
                Builders<UserClaimModel>.Filter.Eq(f => f.Value, claim.Value)));

        using var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await documents.ToListAsync(cancellationToken);
    }

    public Task<bool> HasPasswordAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

    public Task<int> IncrementAccessFailedCountAsync(UserModel user, CancellationToken cancellationToken)
        => Task.FromResult(++user.AccessFailedCount);

    public Task RemoveClaimsAsync(UserModel user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims.Select(claim => new UserClaimModel(claim)))
        {
            user.Claims.Remove(claim);
        }
        return Task.CompletedTask;
    }

    public Task ReplaceClaimAsync(UserModel user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        var userClaim = new UserClaimModel(claim);
        user.Claims.Remove(userClaim);
        user.Claims.Add(new UserClaimModel(newClaim));
        return Task.CompletedTask;
    }

    public Task ResetAccessFailedCountAsync(UserModel user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    public Task SetEmailAsync(UserModel user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email!;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(UserModel user, bool confirmed, CancellationToken cancellationToken)
    {
        user.IsEmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetLockoutEnabledAsync(UserModel user, bool enabled, CancellationToken cancellationToken)
    {
        user.IsLockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetLockoutEndDateAsync(UserModel user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        user.LockoutEndDate = lockoutEnd;
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(UserModel user, string? normalizedEmail, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task SetNormalizedUserNameAsync(UserModel user, string? normalizedName, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task SetPasswordHashAsync(UserModel user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        user.LastPasswordChangedDate = _timeProvider.GetUtcNow();
        return Task.CompletedTask;
    }

    public Task SetSecurityStampAsync(UserModel user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(UserModel user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName!;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(UserModel user, CancellationToken cancellationToken)
    {
        user.ModifiedDate = _timeProvider.GetUtcNow();

        var filter = Builders<UserModel>.Filter.Eq(f => f.Id, user.Id);
        var result = await _collection.ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);
        if (result.IsAcknowledged && (result.ModifiedCount > 0 || result.MatchedCount > 0))
        {
            return IdentityResult.Success;
        }
        return IdentityResult.Failed(_identityErrorDescriber.DefaultError());
    }

    public Task AddToRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
    {
        var claim = new UserClaimModel(ClaimsIdentity.DefaultRoleClaimType, roleName);
        if (!user.Claims.Contains(claim))
        {
            user.Claims.Add(claim);
        }
        return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
    {
        user.Claims.Remove(new UserClaimModel(ClaimsIdentity.DefaultRoleClaimType, roleName));
        return Task.CompletedTask;
    }

    public Task<IList<string>> GetRolesAsync(UserModel user, CancellationToken cancellationToken)
    {
        var roles = user.Claims.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Select(r => r.Value).ToList();
        return Task.FromResult<IList<string>>(roles);
    }

    public Task<bool> IsInRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
    {
        var hasRole = user.Claims.Contains(new UserClaimModel(ClaimsIdentity.DefaultRoleClaimType, roleName));
        return Task.FromResult(hasRole);
    }

    public async Task<IList<UserModel>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var filter = Builders<UserModel>.Filter.ElemMatch(f => f.Claims,
                Builders<UserClaimModel>.Filter.Eq(f => f.Type, ClaimsIdentity.DefaultRoleClaimType));

        using var documents = await _collection.FindAsync(filter, cancellationToken: cancellationToken);
        return await documents.ToListAsync(cancellationToken);
    }
}
