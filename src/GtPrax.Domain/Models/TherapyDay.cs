namespace GtPrax.Domain.Models;

using System;

public sealed class TherapyDay
{
    public bool IsMorning { get; private set; }
    public bool IsAfternoon { get; private set; }
    public bool IsHomeVisit { get; private set; }
    public TimeOnly? AvailableFrom { get; private set; }
    public bool IsAvailableMorning => IsMorning || AvailableFrom?.Hour <= 12;
    public bool IsAvailableAfternoon => IsAfternoon || AvailableFrom?.Hour > 12;

    public TherapyDay(bool isMorning, bool isAfternoon, bool isHomeVisit, TimeOnly? availableFrom)
    {
        IsMorning = isMorning;
        IsAfternoon = isAfternoon;
        IsHomeVisit = isHomeVisit;
        AvailableFrom = availableFrom;
    }
}
