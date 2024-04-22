namespace GtPrax.UI.Pages.WaitingList;

using GtPrax.Application.Converter;
using GtPrax.Application.UseCases.PatientRecord;

public class TherapyDayInput
{
    public bool IsMorning { get; set; }
    public bool IsAfternoon { get; set; }
    public bool IsHomeVisit { get; set; }
    public string? AvailableFrom { get; set; }

    public bool HasValues => IsMorning || IsAfternoon || IsHomeVisit || !string.IsNullOrWhiteSpace(AvailableFrom);

    public TherapyDayDto ToDto(DayOfWeek day, GermanDateTimeConverter dateTimeConverter) =>
        new(day, IsMorning, IsAfternoon, IsHomeVisit, dateTimeConverter.FromIsoTime(AvailableFrom));

    public void FromDto(TherapyDayDto dto, GermanDateTimeConverter dateTimeConverter)
    {
        IsMorning = dto.IsMorning;
        IsAfternoon = dto.IsAfternoon;
        IsHomeVisit = dto.IsHomeVisit;
        AvailableFrom = dto.AvailableFrom is not null ? dateTimeConverter.ToIso(dto.AvailableFrom!.Value) : null;
    }
}
