namespace GtPrax.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

internal static class DbContextExtensions
{
    /// <summary>
    /// Add MySql Provider
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="serverVersion"></param>
    /// <param name="configuration">Access to configuration value: 'ConnectionString'</param>
    public static void AddMySqlContext<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        var assemblyName = typeof(TContext).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<TContext>(options =>
        {
            options.ConfigureWarnings(warn => warn.Ignore(
                CoreEventId.FirstWithoutOrderByAndFilterWarning,
                CoreEventId.RowLimitingOperationWithoutOrderByWarning,
                CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning));

            options.UseMySql(
                configuration.GetConnectionString("MySql"),
                MariaDbServerVersion.LatestSupportedServerVersion,
                mysqlOptions =>
                {
                    mysqlOptions.MaxBatchSize(100);
                    mysqlOptions.MigrationsAssembly(assemblyName);
                });
        });
    }
}
