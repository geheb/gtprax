namespace GtPrax.Infrastructure;

using GtPrax.Application.Email;
using GtPrax.Application.Identity;
using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Identity;
using GtPrax.Infrastructure.Mongo;
using GtPrax.Infrastructure.Worker;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        BsonClassMap.RegisterClassMap<ApplicationUser>(classMap =>
        {
            classMap.AutoMap();
        });

        services.AddTransient<IIdentityService, IdentityService>();

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<ILookupNormalizer, NoneLookupNormalizer>();
        services.AddScoped<IdentityErrorDescriber, GermanyIdentityErrorDescriber>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 10;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(60);
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.AllowedForNewUsers = true;
            options.User.RequireUniqueEmail = true;
        });

        var builder = services
            .AddIdentityCore<ApplicationUser>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider>(ConfirmEmailDataProtectionTokenProviderOptions.ProviderName);

        builder.AddUserStore<ApplicationUserStore>();
        builder.AddSignInManager<SignInManager<ApplicationUser>>();
        builder.Services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();

        services.Configure<MongoConnectionOptions>(config.GetSection("MongoConnection"));
        services.Configure<SmtpConnectionOptions>(config.GetSection("SmtpConnection"));

        services.AddHostedService<HostedWorker>();
        services.AddTransient<MongoConnectionFactory>();
        services.AddTransient<SuperUserService>();
        services.AddTransient<EmailDispatchService>();
        services.AddTransient<EmailQueueStore>();
        services.AddTransient<IEmailSender, SmtpDispatcher>();
        services.AddTransient<IEmailQueueService, EmailQueueService>();

        return services;
    }
}
