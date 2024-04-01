namespace GtPrax.Infrastructure.User;

using System.Threading.Tasks;
using GtPrax.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

internal sealed class AdminService
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _identityService;
    private readonly ILogger _logger;

    public AdminService(
        IConfiguration configuration,
        IUserService identityService,
        ILogger<AdminService> logger)
    {
        _configuration = configuration;
        _identityService = identityService;
        _logger = logger;
    }

    internal async Task Create()
    {
        const string emailKey = "Bootstrap:Admin:Email";
        var email = _configuration[emailKey];
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Bootstrap Admin failed. No configuration for {Key} found!", emailKey);
            return;
        }

        const string passKey = "Bootstrap:Admin:Password";
        var password = _configuration[passKey];
        if (string.IsNullOrEmpty(password))
        {
            _logger.LogWarning("Bootstrap Admin failed. No configuration for {Key} found!", passKey);
            return;
        }

        var result = await _identityService.CreateAdmin(email, password);
        if (result.IsFailed)
        {
            _logger.LogError("Bootstrap Admin failed: {@Errors}", result.Errors);
            return;
        }
    }
}
