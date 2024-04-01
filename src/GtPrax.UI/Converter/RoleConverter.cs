namespace GtPrax.UI.Converter;

using GtPrax.Application.UseCases.UserAccounts;

public sealed class RoleConverter
{
    public string RoleToClass(UserRole role) => role switch
    {
        UserRole.Admin => "is-danger",
        UserRole.Manager => "is-warning",
        UserRole.Staff => "is-info",
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };

    public string RoleToName(UserRole role) => role switch
    {
        UserRole.Admin => "Administrator",
        UserRole.Manager => "Manager",
        UserRole.Staff => "Mitarbeiter",
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };
}
