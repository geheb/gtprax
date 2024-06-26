namespace GtPrax.Application.UseCases.PatientRecord;

public sealed record PatientRecordIndexItemDto(
    string Id,
    DateTimeOffset LastModified,
    string? LastModifiedBy,
    string Name,
    DateOnly BirthDate,
    string PhoneNumber,
    string? ReferralReason,
    PatientRecordTag[] Tags,
    bool HasTherapyDaysWithHomeVisit
);
