namespace GtPrax.Infrastructure.AspNetCore;

using System.ComponentModel.DataAnnotations;

public sealed class RequiredFieldAttribute : RequiredAttribute
{
    public RequiredFieldAttribute()
    {
        ErrorMessage = Messages.FieldIsRequired;
    }
}
