namespace GtPrax.Infrastructure.AspNetCore;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// min 8 and max 100 chars
/// </summary>
public sealed class PasswordLengthFieldAttribute : StringLengthAttribute
{
    public const int MinLen = 10;
    public PasswordLengthFieldAttribute() : base(100)
    {
        MinimumLength = MinLen;
        ErrorMessage = Messages.FieldMissingMinMaxLength;
    }
}
