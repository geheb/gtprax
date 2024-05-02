namespace GtPrax.Application.UseCases.UserAccount;

public sealed record UserTwoFactorDto(bool IsEnabled, string SecretKey, string AuthUri);
