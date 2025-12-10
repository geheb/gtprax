namespace GtPrax.Infrastructure.AspNetCore;

using System.ComponentModel.DataAnnotations;

public sealed class PhoneFieldAttribute : RegularExpressionAttribute
{
    public PhoneFieldAttribute() : base("^(\\d{4,16})$")
    {
        ErrorMessage = Messages.FieldPhoneMismatch;
    }
}
