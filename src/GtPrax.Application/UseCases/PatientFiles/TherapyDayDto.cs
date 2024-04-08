namespace GtPrax.Application.UseCases.PatientFiles;

public sealed record TherapyDayDto(DayOfWeek Day, bool IsMorning, bool IsAfternoon, bool IsHomeVisit, TimeOnly? AvailableFrom);
