namespace GtPrax.Infrastructure;

using GtPrax.Application.Services;
using GtPrax.Domain.Repositories;
using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Mongo;
using GtPrax.Infrastructure.Repositories;
using GtPrax.Infrastructure.User;
using GtPrax.Infrastructure.Worker;
using Microsoft.AspNetCore.Authorization;
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

        BsonClassMap.RegisterClassMap<WaitingListModel>(classMap =>
        {
            classMap.AutoMap();
        });

        BsonClassMap.RegisterClassMap<PatientRecordModel>(classMap =>
        {
            classMap.AutoMap();
        });

        services.AddMemoryCache();

        services.AddScoped<IUserService, UserService>();

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

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(90);
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
        services.AddScoped<UserStore>();
        services.AddScoped<MongoConnectionFactory>();
        services.AddScoped<AdminService>();
        services.AddScoped<EmailDispatchService>();
        services.AddScoped<EmailQueueStore>();
        services.AddScoped<IEmailSender, SmtpDispatcher>();
        services.AddScoped<IEmailQueueService, EmailQueueService>();
        services.AddSingleton<IEmailValidatorService, EmailValidatorService>();
        services.AddScoped<IWaitingListRepo, WaitingListRepo>();
        services.AddScoped<IPatientRecordRepo, PatientRecordRepo>();

        return services;
    }

    public static void AddPolicyTwoFactor(this AuthorizationBuilder builder, string name) =>
        builder.AddPolicy(name, policy => policy.RequireClaim(UserClaims.TwoFactorClaim.Type, UserClaims.TwoFactorClaim.Value));
}
