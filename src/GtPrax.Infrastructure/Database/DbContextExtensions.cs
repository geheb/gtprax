namespace GtPrax.Infrastructure.Database;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal static class DbContextExtensions
{
    public static void AddSqliteContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(configuration.GetConnectionString("Sqlite"));
        var file = new FileInfo(connectionStringBuilder.DataSource);
        if (file.Directory?.Exists == false)
        {
            file.Directory.Create();
        }

        var connectionString = connectionStringBuilder.ToString();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.ConfigureWarnings(warn => warn.Ignore(
                CoreEventId.FirstWithoutOrderByAndFilterWarning,
                CoreEventId.RowLimitingOperationWithoutOrderByWarning,
                CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning));
            options.UseSqlite(connectionString);
        });
    }
}
