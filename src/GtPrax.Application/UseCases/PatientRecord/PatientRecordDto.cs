namespace GtPrax.Application.UseCases.PatientRecord;

public sealed record PatientRecordDto(
    string Name,
    DateOnly BirthDate,
    string PhoneNumber,
    string? ReferralReason,
    string? ReferralDoctor,
    TherapyDayDto[] TherapyDays,
    PatientRecordTag[] Tags,
    string? Remark
);
