namespace GtPrax.Application.UseCases.PatientFiles;


public sealed record PatientFileDto(
    string CreatedBy,
    string Name,
    DateOnly BirthDate,
    string PhoneNumber,
    string? ReferralReason,
    string? ReferralDoctor,
    TherapyDayDto[] TherapyDays,
    PatientFileTag[] Tags,
    string? Remark
);
