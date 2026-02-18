namespace GtPrax.Infrastructure.Worker;

using GtPrax.Infrastructure.Database;
using GtPrax.Infrastructure.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal sealed class HostedWorker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public HostedWorker(
        ILogger<HostedWorker> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    private async Task HandleSuperUser()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var contextInitializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitialiser>();
        await contextInitializer.SeedSuperAdmin();
    }

    private async Task HandleEmails(CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var notificationWorker = scope.ServiceProvider.GetRequiredService<AccountNotificationWorker>();

        try
        {
            await notificationWorker.Execute(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on send emails");
        }
    }

    private async Task HandleMigration(CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _logger.LogInformation("Run database maintenance ...");
        await dbContext.Database.ExecuteSqlRawAsync("VACUUM;", cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("REINDEX;");

        _logger.LogInformation("Optimze database ...");
        await dbContext.Database.ExecuteSqlRawAsync("PRAGMA journal_mode = WAL;");
        await dbContext.Database.ExecuteSqlRawAsync("PRAGMA synchronous = NORMAL;");
        await dbContext.Database.ExecuteSqlRawAsync("PRAGMA cache_size = 10000;");
        await dbContext.Database.ExecuteSqlRawAsync("PRAGMA temp_store = MEMORY;");

        var migrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (migrations.Any())
        {
            _logger.LogInformation("apply pending migrations '{Migrations}'", string.Join(",", migrations));
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await HandleMigration(stoppingToken);

        await HandleSuperUser();

        var rand = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            await HandleEmails(stoppingToken);

            var waitMs = rand.Next(20, 30) * 1000;

            await Task.Delay(waitMs, stoppingToken);
        }
    }
}
