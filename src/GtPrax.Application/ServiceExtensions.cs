namespace GtPrax.Application;

using GtPrax.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<AppOptions>(config.GetSection("App"));

        services.AddMediator(options =>
        {
            options.Namespace = "GtPrax.Application.MediatorGenerated";
            options.ServiceLifetime = ServiceLifetime.Transient;
        });

        return services;
    }
}
