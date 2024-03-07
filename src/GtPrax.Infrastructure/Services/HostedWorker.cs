namespace GtPrax.Infrastructure.Services;

using GtPrax.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal sealed class HostedWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public HostedWorker(
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var superUserService = scope.ServiceProvider.GetRequiredService<SuperUserService>();
            await superUserService.Create();
        }
    }
}
