namespace GtPrax.UI.Attributes;

using System.ComponentModel.DataAnnotations;

public sealed class CompareFieldAttribute : CompareAttribute
{
    public CompareFieldAttribute(string otherProperty)
        : base(otherProperty)
    {
        ErrorMessage = "Die Felder '{0}' und '{1}' stimmen nicht überein.";
    }
}
