namespace GtPrax.Application.UseCases.WaitingLists;


public sealed record PatientFileDto(
    string Issuer,
    string Name,
    DateOnly BirthDate,
    string PhoneNumber,
    string? ReferralReason,
    string? ReferralDoctor,
    TherapyDayDto[] TherapyDays,
    PatientTag[] Tags,
    string? Remark
);
