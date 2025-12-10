namespace GtPrax.Application.Models;

public sealed class TherapyTimesDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsMorning { get; set; }
    public bool IsAfternoon { get; set; }
    public bool IsHomeVisit { get; set; }
    public string? Time { get; set; }
    public bool IsTimeMorning => TimeOnly.TryParse(Time, out var t) && (t.Hour * 100 + t.Minute) < 1200;
    public bool IsTimeAfternoon => TimeOnly.TryParse(Time, out var t) && (t.Hour * 100 + t.Minute) >= 1200;
}
