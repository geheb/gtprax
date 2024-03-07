namespace GtPrax.UI.Attributes;

using System.ComponentModel.DataAnnotations;

public sealed class RangeFieldAttribute : RangeAttribute
{
    private const string Error = "Das Feld '{0}' muss zwischen {1} und {2} liegen.";

    public RangeFieldAttribute(int minimum, int maximum)
        : base(minimum, maximum)
    {
        ErrorMessage = Error;
    }

    public RangeFieldAttribute(double minimum, double maximum)
        : base(minimum, maximum)
    {
        ErrorMessage = Error;
    }
}
