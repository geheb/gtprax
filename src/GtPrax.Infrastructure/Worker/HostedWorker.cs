namespace GtPrax.Infrastructure.Worker;

using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Identity;
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
        await CreateSuperUser();

        while (true)
        {
            await DispatchEmails(stoppingToken);

            await Task.Delay(30000, stoppingToken);
        }
    }

    private async Task CreateSuperUser()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<SuperUserService>();
            await service.Create();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create Super User failed");
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
