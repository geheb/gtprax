namespace GtPrax.Application.Services;

using System.Threading.Tasks;
using FluentResults;
using GtPrax.Domain.Entities;
using GtPrax.Domain.ValueObjects;

public interface IUserService
{
    Task<Result> Create(string email, string name, UserRoleType[] roles);
    Task<Result> CreateAdmin(string email, string password);

    Task<Result> SignIn(string email, string password, CancellationToken cancellationToken);
    Task SignOutCurrent();

    Task<User?> Find(string id);
    Task<User?> FindByEmail(string email);
    Task<User[]> GetAll(CancellationToken cancellationToken);

    Task<Result> SetName(string id, string name, CancellationToken cancellationToken);
    Task<Result> SetEmail(string id, string newEmail);
    Task<Result> SetPassword(string id, string newPassword);
    Task<Result> SetRoles(string id, UserRoleType[] roles);

    Task<string?> GenerateResetPasswordToken(string id);
    Task<Result> VerifyResetPasswordToken(string id, string token);
    Task<Result> ResetPassword(string id, string token, string newPassword);

    Task<string?> GenerateConfirmEmailToken(string id);
    Task<Result> VerifyConfirmEmailToken(string id, string token);
    Task<Result> ConfirmEmail(string id, string token, string newPassword);

    Task<string?> GenerateChangeEmailToken(string id, string newEmail);
    Task<Result> ChangeEmail(string id, string token, string newEmail);

    Task<Result> ChangePassword(string id, string currentPassword, string newPassword);
    Task<Result> CheckPassword(string id, string currentPassword);

    Task<Result> Deactivate(string id);
}
