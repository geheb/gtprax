namespace GtPrax.Infrastructure.Worker;

using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.User;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await CreateAdmin();

        while (true)
        {
            await DispatchEmails(stoppingToken);

            await Task.Delay(30000, stoppingToken);
        }
    }

    private async Task CreateAdmin()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<AdminService>();
            await service.Create();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create Admin failed");
        }
    }

    private async Task DispatchEmails(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<EmailDispatchService>();
            await service.HandleEmails(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle emails failed");
        }
    }
}
