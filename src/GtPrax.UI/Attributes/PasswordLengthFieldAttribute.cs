namespace GtPrax.UI.Attributes;

using System.ComponentModel.DataAnnotations;

public sealed class PasswordLengthFieldAttribute : StringLengthAttribute
{
    public PasswordLengthFieldAttribute()
        : base(100)
    {
        MinimumLength = 10;
        ErrorMessage = "Das Feld '{0}' muss mindestens {2} und höchstens {1} Zeichen enthalten.";
    }
}
