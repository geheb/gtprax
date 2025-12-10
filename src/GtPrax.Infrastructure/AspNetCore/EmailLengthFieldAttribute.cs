namespace GtPrax.Infrastructure.AspNetCore;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// min 7 and max 256 chars
/// </summary>
public sealed class EmailLengthFieldAttribute : StringLengthAttribute
{
    public EmailLengthFieldAttribute() : base(256)
    {
        MinimumLength = 7;
        ErrorMessage = Messages.FieldMissingMinMaxLength;
    }
}
