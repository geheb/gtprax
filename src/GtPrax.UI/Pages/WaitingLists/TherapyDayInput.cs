namespace GtPrax.UI.Pages.WaitingLists;

using System.Globalization;
using GtPrax.Application.UseCases.PatientFiles;

public class TherapyDayInput
{
    public bool IsMorning { get; set; }
    public bool IsAfternoon { get; set; }
    public bool IsHomeVisit { get; set; }
    public string? AvailableFrom { get; set; }

    public bool HasValues => IsMorning || IsAfternoon || IsHomeVisit || !string.IsNullOrWhiteSpace(AvailableFrom);

    public TherapyDayDto ToDto(DayOfWeek day)
    {
        TimeOnly? time = TimeOnly.TryParse(AvailableFrom, CultureInfo.InvariantCulture, out var t) ? t : null;
        return new(day, IsMorning, IsAfternoon, IsHomeVisit, time);
    }
}
