namespace GtPrax.UI.Attributes;

using System.ComponentModel.DataAnnotations;

public sealed class RequiredFieldAttribute : RequiredAttribute
{
    public RequiredFieldAttribute()
    {
        ErrorMessage = "Das Feld '{0}' wird benötigt.";
    }
}
