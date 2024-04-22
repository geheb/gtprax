namespace GtPrax.Application.UseCases.PatientRecord;

public sealed record PatientRecordDto(string WaitingListId, string WaitingListName, PatientRecordItemDto Patient);
