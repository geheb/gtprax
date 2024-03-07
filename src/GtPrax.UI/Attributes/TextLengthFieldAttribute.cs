namespace GtPrax.UI.Attributes;

using System.ComponentModel.DataAnnotations;

public sealed class TextLengthFieldAttribute : StringLengthAttribute
{
    private const string Error = "Das Feld '{0}' muss mindestens {2} und höchstens {1} Zeichen enthalten.";

    public TextLengthFieldAttribute() : base(256)
    {
        MinimumLength = 2;
        ErrorMessage = Error;
    }

    public TextLengthFieldAttribute(int max) : base(max)
    {
        MinimumLength = 2;
        ErrorMessage = Error;
    }
}
