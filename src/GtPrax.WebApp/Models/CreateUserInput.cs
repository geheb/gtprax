namespace GtPrax.WebApp.Models;

using System.ComponentModel.DataAnnotations;
using GtPrax.Application.Models;
using GtPrax.Infrastructure.AspNetCore;

public sealed class CreateUserInput
{
    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    [Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [Display(Name = "Rollen")]
    [RequiredField]
    public bool[] Roles { get; set; } = new bool[3];

    public void ToDto(UserDto dto)
    {
        dto.Name = Name;
        dto.Email = Email;

        var roles = new List<string>();
        if (Roles[0])
        {
            roles.Add(Application.Models.Roles.Admin);
        }

        if (Roles[1])
        {
            roles.Add(Application.Models.Roles.Manager);
        }

        if (Roles[2])
        {
            roles.Add(Application.Models.Roles.Staff);
        }

        dto.Roles = roles.ToArray();
    }
}
