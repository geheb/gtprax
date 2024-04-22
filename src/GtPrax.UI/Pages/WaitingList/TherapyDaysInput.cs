namespace GtPrax.UI.Pages.WaitingList;

using GtPrax.Application.Converter;
using GtPrax.Application.UseCases.PatientRecord;

public sealed class TherapyDaysInput
{
    public TherapyDayInput Monday { get; set; } = new();
    public TherapyDayInput Tuesday { get; set; } = new();
    public TherapyDayInput Wednesday { get; set; } = new();
    public TherapyDayInput Thursday { get; set; } = new();
    public TherapyDayInput Friday { get; set; } = new();

    public TherapyDayDto[] ToDto(GermanDateTimeConverter dateTimeConverter)
    {
        var result = Array.Empty<TherapyDayDto>();
        if (Monday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Monday.ToDto(DayOfWeek.Monday, dateTimeConverter);
        }
        if (Tuesday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Tuesday.ToDto(DayOfWeek.Tuesday, dateTimeConverter);
        }
        if (Wednesday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Wednesday.ToDto(DayOfWeek.Wednesday, dateTimeConverter);
        }
        if (Thursday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Thursday.ToDto(DayOfWeek.Thursday, dateTimeConverter);
        }
        if (Friday.HasValues)
        {
            Array.Resize(ref result, result.Length + 1);
            result[^1] = Friday.ToDto(DayOfWeek.Friday, dateTimeConverter);
        }
        return result;
    }

    internal void FromDto(TherapyDayDto[] days, GermanDateTimeConverter dateTimeConverter)
    {
        var monday = days.FirstOrDefault(d => d.Day == DayOfWeek.Monday);
        if (monday is not null)
        {
            Monday.FromDto(monday, dateTimeConverter);
        }
        var tuesday = days.FirstOrDefault(d => d.Day == DayOfWeek.Tuesday);
        if (tuesday is not null)
        {
            Tuesday.FromDto(tuesday, dateTimeConverter);
        }
        var wednesday = days.FirstOrDefault(d => d.Day == DayOfWeek.Wednesday);
        if (wednesday is not null)
        {
            Wednesday.FromDto(wednesday, dateTimeConverter);
        }
        var thursday = days.FirstOrDefault(d => d.Day == DayOfWeek.Thursday);
        if (thursday is not null)
        {
            Thursday.FromDto(thursday, dateTimeConverter);
        }
        var friday = days.FirstOrDefault(d => d.Day == DayOfWeek.Friday);
        if (friday is not null)
        {
            Friday.FromDto(friday, dateTimeConverter);
        }
    }
}
