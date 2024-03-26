namespace GtPrax.Infrastructure.Identity;

using System.Threading.Tasks;
using GtPrax.Application.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

internal sealed class SuperUserService
{
    private readonly IConfiguration _configuration;
    private readonly IIdentityService _identityService;
    private readonly ILogger _logger;

    public SuperUserService(
        IConfiguration configuration,
        IIdentityService identityService,
        ILogger<SuperUserService> logger)
    {
        _configuration = configuration;
        _identityService = identityService;
        _logger = logger;
    }

    internal async Task Create()
    {
        const string emailKey = "Bootstrap:SuperUser:Email";
        var superUserEmail = _configuration[emailKey];
        if (string.IsNullOrEmpty(superUserEmail))
        {
            _logger.LogError("Bootstrap super user failed. No configuration for {Key} found!", emailKey);
            return;
        }

        const string passKey = "Bootstrap:SuperUser:Password";
        var password = _configuration[passKey];
        if (string.IsNullOrEmpty(password))
        {
            _logger.LogError("Bootstrap super user failed. No configuration for {Key} found!", passKey);
            return;
        }

        var result = await _identityService.CreateSuperUser(superUserEmail, password);
        if (!result.Succeeded)
        {
            _logger.LogError("Create super user failed: {@Errors}", result.Errors);
            return;
        }
    }
}
