namespace GtPrax.Application.UseCases.PatientRecord;

public sealed record TherapyDayDto(DayOfWeek Day, bool IsMorning, bool IsAfternoon, bool IsHomeVisit, TimeOnly? AvailableFrom);
