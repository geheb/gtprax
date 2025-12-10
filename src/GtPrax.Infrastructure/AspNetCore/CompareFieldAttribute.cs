namespace GtPrax.Infrastructure.AspNetCore;

using System.ComponentModel.DataAnnotations;

public sealed class CompareFieldAttribute : CompareAttribute
{
    public CompareFieldAttribute(string otherProperty) : base(otherProperty)
    {
        ErrorMessage = Messages.FieldsNotMatch;
    }
}
