namespace GtPrax.UI.Pages.UserAccounts;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.UseCases.UserAccounts;
using GtPrax.UI.Attributes;

public sealed class CreateInput
{
    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    [Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailField]
    public string? Email { get; set; }

    [Display(Name = "Rollen")]
    [RequiredField]
    public bool[] Roles { get; set; } = new bool[3];

    public UserRole[] GetRoles()
    {
        var count = Roles.Count(r => r);
        var index = 0;
        var result = new UserRole[count];
        if (Roles[0])
        {
            result[index++] = UserRole.Staff;
        }
        if (Roles[1])
        {
            result[index++] = UserRole.Admin;
        }
        if (Roles[2])
        {
            result[index++] = UserRole.Manager;
        }
        return result;
    }
}
