using System.Net;

namespace GtPrax.Infrastructure.Security;

internal interface IIpReputationChecker
{
    Task<bool> IsListed(IPAddress address);
    Task<bool> IsListedMx(string domain, CancellationToken cancellationToken);
}
