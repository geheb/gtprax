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
        BsonClassMap.RegisterClassMap<UserModel>(classMap =>
        {
            classMap.AutoMap();
        });

        services.AddMemoryCache();

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
            options.Password.RequiredUniqueChars = 5;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(60);
            options.Lockout.MaxFailedAccessAttempts = 3;

            options.Lockout.AllowedForNewUsers = true;
            options.User.RequireUniqueEmail = true;
        });

        var builder = services
            .AddIdentityCore<UserModel>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider>(ConfirmEmailDataProtectionTokenProviderOptions.ProviderName);

        builder.AddUserStore<UserStore>();
        builder.AddSignInManager<SignInManager<UserModel>>();
        builder.Services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<UserModel>>();

        services.Configure<MongoConnectionOptions>(config.GetSection("MongoConnection"));
        services.Configure<SmtpConnectionOptions>(config.GetSection("SmtpConnection"));

        services.AddHostedService<HostedWorker>();
        services.AddTransient<UserStore>();
        services.AddTransient<MongoConnectionFactory>();
        services.AddTransient<SuperUserService>();
        services.AddTransient<EmailDispatchService>();
        services.AddTransient<EmailQueueStore>();
        services.AddTransient<IEmailSender, SmtpDispatcher>();
        services.AddTransient<IEmailQueueService, EmailQueueService>();
        services.AddSingleton<IEmailValidatorService, EmailValidatorService>();

        return services;
    }
}
