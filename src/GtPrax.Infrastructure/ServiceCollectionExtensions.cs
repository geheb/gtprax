namespace GtPrax.Infrastructure;

using GtPrax.Application.Repositories;
using GtPrax.Application.Services;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.Infrastructure.Database;
using GtPrax.Infrastructure.Database.Entities;
using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Identity;
using GtPrax.Infrastructure.Repositories;
using GtPrax.Infrastructure.Worker;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void RegisterCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqliteContext(configuration);

        services.AddIdentity<IdentityUserGuid, IdentityRoleGuid>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.Tokens.EmailConfirmationTokenProvider = ConfirmEmailDataProtectionTokenProviderOptions.ProviderName;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<IdentityUserGuid>>(ConfirmEmailDataProtectionTokenProviderOptions.ProviderName);

        services.AddScoped<AppDbContextInitialiser>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWaitlistRepository, WaitlistRepository>();
        services.AddScoped<AccountNotificationWorker>();
        services.AddHostedService<HostedWorker>();

        services.AddSingleton<IpReputationChecker>();
        services.AddSingleton<INodeGenerator, NodeGenerator>();
        services.AddSingleton<IEmailValidator, EmailValidator>();
        services.AddSingleton<EmailSender>();
    }
}
