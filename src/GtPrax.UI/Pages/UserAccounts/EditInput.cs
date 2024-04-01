namespace GtPrax.UI.Pages.UserAccounts;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.UseCases.UserAccounts;
using GtPrax.UI.Attributes;

public sealed class EditInput
{
    [Display(Name = "Name")]
    [TextLengthField]
    public string? Name { get; set; }

    [Display(Name = "E-Mail-Adresse")]
    [EmailField]
    public string? Email { get; set; }

    [Display(Name = "Passwort")]
    [PasswordLengthField]
    public string? Password { get; set; }

    [Display(Name = "Rollen")]
    public bool[] Roles { get; set; } = new bool[3];

    public void Set(UserDto user)
    {
        Name = user.Name;
        Email = user.Email;
        Roles[0] = user.Roles.Contains(UserRole.Staff);
        Roles[1] = user.Roles.Contains(UserRole.Admin);
        Roles[2] = user.Roles.Contains(UserRole.Manager);
    }

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
