namespace GtPrax.Domain.Entities;

public sealed class TherapyDays
{
    public TherapyDay? Monday { get; private set; }
    public TherapyDay? Tuesday { get; private set; }
    public TherapyDay? Wednesday { get; private set; }
    public TherapyDay? Thursday { get; private set; }
    public TherapyDay? Friday { get; private set; }
}
