namespace GtPrax.Application.UseCases.PatientRecord;

public sealed record PatientRecordItemDto(
    DateTimeOffset Created,
    DateTimeOffset? LastModified,
    string? LastModifiedBy,
    string Name,
    DateOnly BirthDate,
    string PhoneNumber,
    string? ReferralReason,
    string? ReferralDoctor,
    TherapyDayDto[] TherapyDays,
    PatientRecordTag[] Tags,
    string? Remark
);
