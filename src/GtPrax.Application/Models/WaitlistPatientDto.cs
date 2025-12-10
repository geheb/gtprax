namespace GtPrax.Application.Models;

public sealed class WaitlistPatientDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? User { get; set; }
    public Guid? WaitlistId { get; set; }
    public DateTimeOffset? Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
    public string? Name { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Reason { get; set; }
    public string? Doctor { get; set; }
    public TherapyTimesDto[]? TherapyTimes { get; set; }
    public string? Remark { get; set; }
    public TagsDto? Tags { get; set; }

    public bool HasTherapyTimesMorning => TherapyTimes == null || TherapyTimes.Any(t => t.IsMorning || t.IsTimeMorning);
    public bool HasTherapyTimesAfternoon => TherapyTimes == null || TherapyTimes.Any(t => t.IsAfternoon || t.IsTimeAfternoon);
    public bool HasTherapyTimesHomeVisit => TherapyTimes != null && TherapyTimes.Any(t => t.IsHomeVisit);

    public int CalcAge(DateTime today)
    {
        var age = today.Year - Birthday!.Value.Year;
        var birthdayTime = Birthday.Value.ToDateTime(TimeOnly.MinValue);
        // Go back to the year in which the person was born in case of a leap year
        if (birthdayTime > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
