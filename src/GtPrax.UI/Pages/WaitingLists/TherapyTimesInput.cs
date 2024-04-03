namespace GtPrax.UI.Pages.WaitingLists;

public class TherapyTimesInput
{
    public TherapyDayInput Monday { get; set; } = new();
    public TherapyDayInput Tuesday { get; set; } = new();
    public TherapyDayInput Wednesday { get; set; } = new();
    public TherapyDayInput Thursday { get; set; } = new();
    public TherapyDayInput Friday { get; set; } = new();
}
