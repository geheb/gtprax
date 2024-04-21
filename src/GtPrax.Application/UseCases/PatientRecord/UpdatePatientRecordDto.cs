namespace GtPrax.Application.UseCases.PatientRecord;


public sealed record UpdatePatientRecordDto(
    string PhoneNumber,
    string? ReferralReason,
    string? ReferralDoctor,
    TherapyDayDto[] TherapyDays,
    PatientRecordTag[] Tags,
    string? Remark
);
