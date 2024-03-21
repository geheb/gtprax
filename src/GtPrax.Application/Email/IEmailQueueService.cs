namespace GtPrax.Application.Email;

using System.Threading.Tasks;

public interface IEmailQueueService
{
    Task EnqueueConfirmEmail(string email, string name, string link, CancellationToken cancellationToken);
    Task EnqueueResetPassword(string email, string name, string link, CancellationToken cancellationToken);
    Task EnqueueChangeEmail(string email, string name, string link, CancellationToken cancellationToken);
}
