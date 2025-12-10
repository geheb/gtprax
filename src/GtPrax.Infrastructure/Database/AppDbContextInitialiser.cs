namespace GtPrax.Infrastructure.Database;

using GtPrax.Application.Models;
using GtPrax.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

internal sealed class AppDbContextInitialiser
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUserGuid> _userManager;

    public AppDbContextInitialiser(
        AppDbContext dbContext,
        IConfiguration configuration,
        UserManager<IdentityUserGuid> userManager)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task SeedSuperAdmin()
    {
        const string emailKey = "Bootstrap:SuperUser:Email";
        var superUserEmail = _configuration[emailKey];
        if (string.IsNullOrEmpty(superUserEmail))
        {
            throw new InvalidProgramException(emailKey);
        }

        var superUser = await _userManager.FindByEmailAsync(superUserEmail);

        if (superUser != null)
        {
            return;
        }

        var superUserName = _configuration["Bootstrap:SuperUser:Name"];

        var id = _dbContext.GeneratePk();

        superUser = new IdentityUserGuid
        {
            Id = id,
            Name = superUserName,
            UserName = id.ToString("N"),
            Email = superUserEmail,
            EmailConfirmed = true,
        };

        const string passKey = "Bootstrap:SuperUser:Password";
        var password = _configuration[passKey];
        if (string.IsNullOrEmpty(password))
        {
            throw new InvalidProgramException(passKey);
        }

        var result = await _userManager.CreateAsync(superUser, password);
        if (!result.Succeeded)
        {
            throw new InvalidProgramException("Add super user failed: " + result);
        }

        result = await _userManager.AddToRolesAsync(superUser, [Roles.Admin, Roles.Manager, Roles.Staff]);
        if (!result.Succeeded)
        {
            throw new InvalidProgramException("Add super user roles failed: " + result);
        }
    }
}
