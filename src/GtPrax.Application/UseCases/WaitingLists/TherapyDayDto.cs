namespace GtPrax.Application.UseCases.WaitingLists;

public sealed record TherapyDayDto(DayOfWeek Day, bool IsMorning, bool IsAfternoon, bool IsHomeVisit, TimeOnly? AvailableFrom);
