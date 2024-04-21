namespace GtPrax.Application.UseCases.PatientRecord;

public sealed record PatientRecordIndexDto(string WaitingListName, PatientRecordIndexItemDto[] Items);
