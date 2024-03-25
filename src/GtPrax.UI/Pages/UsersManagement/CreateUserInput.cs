namespace GtPrax.UI.Pages.UsersManagement;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Identity;
using GtPrax.Application.UseCases.UsersManagement;
using GtPrax.UI.Attributes;

public sealed class CreateUserInput
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

    public void Set(UserDto user)
    {
        Name = user.Name;
        Email = user.Email;
        Roles[0] = user.Roles.Contains(UserRole.Staff);
        Roles[1] = user.Roles.Contains(UserRole.Manager);
        Roles[2] = user.Roles.Contains(UserRole.Admin);
    }
}
