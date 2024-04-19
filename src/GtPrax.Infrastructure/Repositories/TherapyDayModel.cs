namespace GtPrax.Infrastructure.Repositories;

using System;

internal sealed class TherapyDayModel
{
    public DayOfWeek Day { get; set; }
    public bool IsMorning { get; set; }
    public bool IsAfternoon { get; set; }
    public bool IsHomeVisit { get; set; }
    public string? AvailableFrom { get; set; }
}
