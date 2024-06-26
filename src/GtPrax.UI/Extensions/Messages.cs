namespace GtPrax.UI.Extensions;

public static class Messages
{
    public const string InvalidRequest = "Die Anfrage ist ungültig.";
    public const string SignInFailed = "Die Anmeldung ist fehlgeschlagen.";
    public const string ResetPasswordSent = "Du bekommst demnächst eine E-Mail von uns! In der E-Mail ist ein Link zum Zurücksetzen des Passworts zu finden.";
    public const string ChangeEmailSent = "Du bekommst demnächst eine E-Mail von uns! In der E-Mail ist ein Link zum Bestätigen der E-Mail-Adresse.";
    public const string PasswordChanged = "Das Passwort wurde geändert. Melde dich jetzt mit dem neuen Passwort an.";
    public const string EmailConfirmed = "Deine E-Mail-Adresse wurde bestätigt. Melde dich jetzt an.";
    public const string UserNotFound = "Der Benutzer wurde nicht gefunden.";
    public const string ChangesSaved = "Die Änderungen wurden gespeichert.";
    public const string PasswordSaved = "Das Passwort wurde geändert.";
    public const string ProcessRequestFailed = "Die Anfrage kann nicht bearbeitet werden, es wurden ungültige Daten gesendet.";
    public static string PageAccessDenied(string? page) => $"Der Zugriff auf die angeforderte Seite '{page}' wurde verweigert.";
    public const string PageNotFound = "Die angeforderte Seite wurde nicht gefunden.";
    public const string InternalServerError = "Es ist ein interner Server-Fehler aufgetreten.";
    public const string RoleRequired = "Mindestens eine Rolle wird benötigt.";
    public const string InvalidConfirmEmailToken = "Der Bestätigungslink für die E-Mail-Adresse ist leider unültig bzw. bereits abgelaufen.";
    public const string InvalidResetPasswordToken = "Der Link zum Zurücksetzen des Passworts ist leider unültig bzw. bereits abgelaufen.";
    public const string InvalidChangeEmailToken = "Der Bestätigungslink für die Änderung der E-Mail-Adresse ist leider unültig bzw. bereits abgelaufen.";
    public const string TwoFactorEnabled = "Zwei-Faktor-Authentifizierung (2FA) wurde aktiviert.";
    public const string TwoFactorDisabled = "Zwei-Faktor-Authentifizierung (2FA) wurde deaktiviert.";
}
