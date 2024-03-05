using GtPrax.Application.Identity;
using GtPrax.Infrastructure.Identity;
using GtPrax.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization;

namespace GtPrax.Infrastructure;

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
			options.Password.RequiredLength = 8;
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
			.AddDefaultTokenProviders();

		builder.AddUserStore<ApplicationUserStore>();
		builder.AddSignInManager<SignInManager<ApplicationUser>>();
		builder.Services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();

		services.AddScoped<MongoConnectionFactory>();

		services.Configure<MongoConnectionOptions>(config.GetSection("MongoConnection"));

		return services;
	}
}
