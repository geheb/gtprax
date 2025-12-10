namespace GtPrax.Application.Repositories;

using GtPrax.Application.Models;

public interface IUserRepository
{
    Task<string?> SignIn(string email, string password);
    Task SignOutCurrentUser();
    Task<bool> NotifyPasswordForgotten(string email, CancellationToken cancellationToken);
    Task<string[]?> Update(UserDto dto, string? password, CancellationToken cancellationToken);
    Task<string[]?> Update(Guid id, string name);
    Task<string?> VerfiyChangePassword(Guid id, string token);
    Task<(string[]? Error, string? Email)> ChangePassword(Guid id, string? token, string password);
    Task<(string? Error, bool IsFatal)> NotifyConfirmChangeEmail(Guid id, string newEmail, string currentPassword, CancellationToken cancellationToken);
    Task<string?> VerifyConfirmRegistration(Guid id, string token);
    Task<string[]?> ConfirmRegistrationAndSetPassword(Guid id, string token, string password);
    Task<bool> NotifyConfirmRegistration(Guid id, CancellationToken cancellationToken);
    Task<UserDto[]> GetAll(CancellationToken cancellationToken);
    Task<UserDto?> Find(Guid id, CancellationToken cancellationToken);
    Task<string[]?> Create(UserDto dto, CancellationToken cancellationToken);
    Task<string?> ConfirmChangeEmail(Guid id, string token, string encodedEmail);
    Task<bool> Remove(Guid id, CancellationToken cancellationToken);
}
