namespace GtPrax.Infrastructure.Identity;

using System.Threading.Tasks;
using GtPrax.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

internal sealed class SuperUserService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger _logger;

    public SuperUserService(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        ILogger<SuperUserService> logger)
    {
        _configuration = configuration;
        _userManager = userManager;
        _logger = logger;
    }

    internal async Task Create()
    {
        const string emailKey = "Bootstrap:SuperUser:Email";
        var superUserEmail = _configuration[emailKey];
        if (string.IsNullOrEmpty(superUserEmail))
        {
            _logger.LogError("Boostrap super user failed. No configuration for {Key} found!", emailKey);
            return;
        }

        const string passKey = "Bootstrap:SuperUser:Password";
        var password = _configuration[passKey];
        if (string.IsNullOrEmpty(password))
        {
            _logger.LogError("Boostrap super user failed. No configuration for {Key} found!", passKey);
            return;
        }

        var superUser = await _userManager.FindByEmailAsync(superUserEmail);
        if (superUser != null)
        {
            return;
        }

        superUser = new ApplicationUser
        {
            Id = ObjectId.GenerateNewId(),
            Name = "Super User",
            UserName = "SuperUser",
            Email = superUserEmail,
            EmailConfirmed = true,
        };
        superUser.Roles.Add(new ApplicationUserRole(UserRole.Admin.ToString()));

        var result = await _userManager.CreateAsync(superUser, password);
        if (!result.Succeeded)
        {
            _logger.LogError("Create super user failed: {Errors}", string.Join(",", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
            return;
        }
    }
}
