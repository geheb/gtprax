using Microsoft.AspNetCore.Identity;

namespace GtPrax.Infrastructure.Identity;

internal sealed class GermanyIdentityErrorDescriber : IdentityErrorDescriber
{
	public override IdentityError DefaultError() => new IdentityError { Code = nameof(DefaultError), Description = $"Ein unbekannter Fehler ist aufgetreten." };
	public override IdentityError ConcurrencyFailure() => new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Fehler bzgl. der optimistischen Nebenläufigkeit. Das Objekt wurde verändert." };
	public override IdentityError PasswordMismatch() => new IdentityError { Code = nameof(PasswordMismatch), Description = "Ungültiges Passwort." };
	public override IdentityError InvalidToken() => new IdentityError { Code = nameof(InvalidToken), Description = "Ungültiges Token." };
	public override IdentityError RecoveryCodeRedemptionFailed() => new IdentityError { Code = nameof(RecoveryCodeRedemptionFailed), Description = "Die Einlösung des Wiederherstellungscodes ist fehlgeschlagen." };
	public override IdentityError LoginAlreadyAssociated() => new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Es ist bereits ein Nutzer mit diesem Login vorhanden." };
	public override IdentityError InvalidUserName(string? userName) => new IdentityError { Code = nameof(InvalidUserName), Description = $"Nutzername '{userName}' ist ungültig. Erlaubt sind nur Buchstaben und Zahlen." };
	public override IdentityError InvalidEmail(string? email) => new IdentityError { Code = nameof(InvalidEmail), Description = $"E-Mail '{email}' ist ungültig." };
	public override IdentityError DuplicateUserName(string? userName) => new IdentityError { Code = nameof(DuplicateUserName), Description = $"Nutzername '{userName}' ist bereits vergeben." };
	public override IdentityError DuplicateEmail(string? email) => new IdentityError { Code = nameof(DuplicateEmail), Description = $"E-Mail '{email}' ist bereits vergeben." };
	public override IdentityError InvalidRoleName(string? role) => new IdentityError { Code = nameof(InvalidRoleName), Description = $"Rollen-Name '{role}' ist ungültig." };
	public override IdentityError DuplicateRoleName(string? role) => new IdentityError { Code = nameof(DuplicateRoleName), Description = $"Rollen-Name '{role}' ist bereits vergeben." };
	public override IdentityError UserAlreadyHasPassword() => new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Nutzer hat bereits ein Passwort gesetzt." };
	public override IdentityError UserLockoutNotEnabled() => new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Aussperrung ist für diesen Nutzer nicht aktiviert." };
	public override IdentityError UserAlreadyInRole(string? role) => new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"Nutzer hat bereits die Rolle '{role}'." };
	public override IdentityError UserNotInRole(string? role) => new IdentityError { Code = nameof(UserNotInRole), Description = $"Nutzer ist nicht in der Rolle '{role}'." };
	public override IdentityError PasswordTooShort(int length) => new IdentityError { Code = nameof(PasswordTooShort), Description = $"Passwörter müssen mindestens {length} Zeichen lang sein." };
	public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new IdentityError { Code = nameof(PasswordRequiresUniqueChars), Description = $"Passwörter müssen mindestens {uniqueChars} verschiedene Zeichen enthalten." };
	public override IdentityError PasswordRequiresNonAlphanumeric() => new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Passwörter müssen mindestens ein Sonderzeichen enthalten." };
	public override IdentityError PasswordRequiresDigit() => new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Passwörter müssen mindestens eine Ziffer enthalten ('0'-'9')." };
	public override IdentityError PasswordRequiresLower() => new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Passwörter müssen mindestens einen Kleinbuchstaben enthalten ('a'-'z')." };
	public override IdentityError PasswordRequiresUpper() => new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Passwörter müssen mindestens einen Großbuchstaben enthalten ('A'-'Z')." };
}

