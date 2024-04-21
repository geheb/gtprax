namespace GtPrax.Application.UseCases.PatientRecord;

public sealed record PatientRecordIndexItemDto(
    string Id,
    DateTimeOffset LastModified,
    string? LastModifedBy,
    string Name,
    DateOnly BirthDate,
    string PhoneNumber,
    string? ReferralReason,
    PatientRecordTag[] Tags,
    bool HasTherapyDaysWithHomeVisit
);
