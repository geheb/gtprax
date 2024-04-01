namespace GtPrax.Domain.Entities;

using System;

public sealed class TherapyDay
{
    public bool IsMorning { get; private set; }
    public bool IsAfternoon { get; private set; }
    public bool IsHomeVisit { get; private set; }
    public TimeOnly? AvailableFrom { get; private set; }
}
