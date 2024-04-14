namespace GtPrax.Application.UseCases.PatientFiles;


public sealed record UpdatePatientFileDto(
    string PhoneNumber,
    string? ReferralReason,
    string? ReferralDoctor,
    TherapyDayDto[] TherapyDays,
    PatientFileTag[] Tags,
    string? Remark
);
