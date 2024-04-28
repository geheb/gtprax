namespace GtPrax.Application.UseCases.PatientRecord;

public sealed record PatientRecordIndexDto(PatientRecordIndexItemDto[] Items, int TotalCount);
