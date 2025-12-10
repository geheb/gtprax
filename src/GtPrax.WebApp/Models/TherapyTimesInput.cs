namespace GtPrax.WebApp.Models;

using GtPrax.Application.Models;

public class TherapyTimesInput
{
    public bool[] Monday { get; set; } = new bool[3];
    public string? MondayTime { get; set; }
    public bool[] Tuesday { get; set; } = new bool[3];
    public string? TuesdayTime { get; set; }
    public bool[] Wednesday { get; set; } = new bool[3];
    public string? WednesdayTime { get; set; }
    public bool[] Thursday { get; set; } = new bool[3];
    public string? ThursdayTime { get; set; }
    public bool[] Friday { get; set; } = new bool[3];
    public string? FridayTime { get; set; }

    public TherapyTimesDto[] Map()
    {
        var items = new List<TherapyTimesDto>();
        if (Monday.Any(b => b) || !string.IsNullOrEmpty(MondayTime))
        {
            items.Add(Create(DayOfWeek.Monday, Monday, MondayTime));
        }
        if (Tuesday.Any(b => b) || !string.IsNullOrEmpty(TuesdayTime))
        {
            items.Add(Create(DayOfWeek.Tuesday, Tuesday, TuesdayTime));
        }
        if (Wednesday.Any(b => b) || !string.IsNullOrEmpty(WednesdayTime))
        {
            items.Add(Create(DayOfWeek.Wednesday, Wednesday, WednesdayTime));
        }
        if (Thursday.Any(b => b) || !string.IsNullOrEmpty(ThursdayTime))
        {
            items.Add(Create(DayOfWeek.Thursday, Thursday, ThursdayTime));
        }
        if (Friday.Any(b => b) || !string.IsNullOrEmpty(FridayTime))
        {
            items.Add(Create(DayOfWeek.Friday, Friday, FridayTime));
        }
        return items.ToArray();
    }

    public void Map(TherapyTimesDto[]? dto)
    {
        Array.Fill(Monday, false);
        MondayTime = default;
        Array.Fill(Tuesday, false);
        TuesdayTime = default;
        Array.Fill(Wednesday, false);
        WednesdayTime = default;
        Array.Fill(Thursday, false);
        ThursdayTime = default;
        Array.Fill(Friday, false);
        FridayTime = default;

        if (dto == null || dto.Length == 0)
        {
            return;
        }

        foreach (var d in dto)
        {
            switch (d.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    Monday[0] = d.IsMorning;
                    Monday[1] = d.IsAfternoon;
                    Monday[2] = d.IsHomeVisit;
                    MondayTime = d.Time;
                    break;
                case DayOfWeek.Tuesday:
                    Tuesday[0] = d.IsMorning;
                    Tuesday[1] = d.IsAfternoon;
                    Tuesday[2] = d.IsHomeVisit;
                    TuesdayTime = d.Time;
                    break;
                case DayOfWeek.Wednesday:
                    Wednesday[0] = d.IsMorning;
                    Wednesday[1] = d.IsAfternoon;
                    Wednesday[2] = d.IsHomeVisit;
                    WednesdayTime = d.Time;
                    break;
                case DayOfWeek.Thursday:
                    Thursday[0] = d.IsMorning;
                    Thursday[1] = d.IsAfternoon;
                    Thursday[2] = d.IsHomeVisit;
                    ThursdayTime = d.Time;
                    break;
                case DayOfWeek.Friday:
                    Friday[0] = d.IsMorning;
                    Friday[1] = d.IsAfternoon;
                    Friday[2] = d.IsHomeVisit;
                    FridayTime = d.Time;
                    break;
            }
        }
    }

    private TherapyTimesDto Create(DayOfWeek dayOfWeek, bool[] day, string? time) =>
        new()
        {
            DayOfWeek = dayOfWeek,
            IsMorning = day[0],
            IsAfternoon = day[1],
            IsHomeVisit = day[2],
            Time = time != null ? (TimeOnly.TryParse(time, out var t) ? t.ToShortTimeString() : null) : null
        };
}
