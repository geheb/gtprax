namespace GtPrax.Infrastructure.AspNetCore;

using System.ComponentModel.DataAnnotations;

public sealed class RangeFieldAttribute : RangeAttribute
{
    public RangeFieldAttribute(int minimum, int maximum) 
        : base(minimum, maximum)
    {
        ErrorMessage = Messages.FieldNotInRange;
    }

    public RangeFieldAttribute(double minimum, double maximum)
        : base(minimum, maximum)
    {
        ErrorMessage = Messages.FieldNotInRange;
    }
}
