namespace GtPrax.Infrastructure.AspNetCore;

using System.ComponentModel.DataAnnotations;

public sealed class TextLengthFieldAttribute : StringLengthAttribute
{
    public TextLengthFieldAttribute() : base(256)
    {
        MinimumLength = 2;
        ErrorMessage = Messages.FieldMissingMinMaxLength;
    }

    public TextLengthFieldAttribute(int max) : base(max)
    {
        MinimumLength = 2;
        ErrorMessage = Messages.FieldMissingMinMaxLength;
    }
}
