namespace GtPrax.UI.Pages.WaitingLists;

using GtPrax.Application.UseCases.PatientFiles;

public class TherapyTimesInput
{
    public TherapyDayInput Monday { get; set; } = new();
    public TherapyDayInput Tuesday { get; set; } = new();
    public TherapyDayInput Wednesday { get; set; } = new();
    public TherapyDayInput Thursday { get; set; } = new();
    public TherapyDayInput Friday { get; set; } = new();

    public TherapyDayDto[] ToDto()
    {
        var result = Array.Empty<TherapyDayDto>();
        if (Monday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Monday.ToDto(DayOfWeek.Monday);
        }
        if (Tuesday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Tuesday.ToDto(DayOfWeek.Tuesday);
        }
        if (Wednesday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Wednesday.ToDto(DayOfWeek.Wednesday);
        }
        if (Thursday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Thursday.ToDto(DayOfWeek.Thursday);
        }
        if (Friday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Friday.ToDto(DayOfWeek.Friday);
        }
        return result;
    }
}
