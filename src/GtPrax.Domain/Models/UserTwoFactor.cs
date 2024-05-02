namespace GtPrax.Domain.Models;

public sealed record UserTwoFactor(bool IsEnabled, string SecretKey, string AuthUri);
