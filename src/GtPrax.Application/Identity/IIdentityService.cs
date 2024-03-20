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

    Task<string?> GenerateConfirmEmailToken(string id);
    Task<IdentityResult> ConfirmEmail(string id, string token, string newPassword);

    Task<string?> GenerateChangeEmailToken(string id, string newEmail);
    Task<IdentityResult> ChangeEmail(string id, string token, string newEmail);

    Task<IdentityResult> ChangePassword(string id, string currentPassword, string newPassword);
    Task<IdentityResult> CheckPassword(string id, string currentPassword);
}
