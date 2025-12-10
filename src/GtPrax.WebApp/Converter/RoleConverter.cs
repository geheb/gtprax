namespace GtPrax.WebApp.Converter;

using GtPrax.Application.Models;

public sealed class RoleConverter
{
    public string RoleToClass(string role) =>
        role switch
        {
            Roles.Admin => "is-danger",
            Roles.Manager => "is-warning",
            Roles.Staff => "is-info",
            _ => string.Empty
        };

    public string RoleToName(string role) =>
        role switch
        {
            Roles.Admin => "Administrator",
            Roles.Manager => "Manager",
            Roles.Staff => "Mitarbeiter",
            _ => string.Empty
        };
}
