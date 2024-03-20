namespace GtPrax.Application.Email;

public interface IEmailValidatorService
{
    Task<bool> Validate(string email, CancellationToken cancellationToken);
}
