namespace GtPrax.WebApp.Models;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Models;
using GtPrax.Infrastructure.AspNetCore;

public sealed class EditUserInput
{
    private const int AdminIndex = 0;
    private const int ManagerIndex = 1;
    private const int StaffIndex = 2;

    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    [Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [Display(Name = "Passwort")]
    [PasswordLengthField]
    public string? Password { get; set; }

    [Display(Name = "Rollen")]
    [RequiredField]
    public bool[] Roles { get; set; } = new bool[3];

    public void FromDto(UserDto dto)
    {
        Name = dto.Name;
        Email = dto.Email;
        if (dto.Roles != null)
        {
            if (dto.Roles.Any(r => r == Application.Models.Roles.Admin))
            {
                Roles[AdminIndex] = true;
            }

            if (dto.Roles.Any(r => r == Application.Models.Roles.Manager))
            {
                Roles[ManagerIndex] = true;
            }

            if (dto.Roles.Any(r => r == Application.Models.Roles.Staff))
            {
                Roles[StaffIndex] = true;
            }
        }
    }

    public void ToDto(UserDto dto)
    {
        dto.Name = Name;
        dto.Email = Email;

        var roles = new List<string>();
        if (Roles[AdminIndex])
        {
            roles.Add(Application.Models.Roles.Admin);
        }

        if (Roles[ManagerIndex])
        {
            roles.Add(Application.Models.Roles.Manager);
        }

        if (Roles[StaffIndex])
        {
            roles.Add(Application.Models.Roles.Staff);
        }

        dto.Roles = roles.ToArray();
    }
}
