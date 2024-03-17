namespace GtPrax.Application.Identity;

using System.Threading.Tasks;
using GtPrax.Domain.Entities;
using Microsoft.AspNetCore.Identity;

public interface IIdentityService
{
    Task<IdentityResult> SignIn(string email, string password, CancellationToken cancellationToken);
    Task SignOutCurrentUser();
    Task<User?> FindUser(string id);
    Task<User[]> GetAllUsers(CancellationToken cancellationToken);
    Task<IdentityResult> SetName(string id, string name, CancellationToken cancellationToken);
    Task<IdentityResult> ResetPassword(string id, string password);
    Task<IdentityResult> SetRoles(string id, UserRole[] roles);
    Task<string?> GeneratePasswordResetToken(string id);
    Task<string?> GenerateEmailConfirmationToken(string id);
    Task<IdentityResult> VerifyPasswordResetToken(string id, string token);
    Task<IdentityResult> VerifyEmailConfirmationToken(string id, string token);
}
