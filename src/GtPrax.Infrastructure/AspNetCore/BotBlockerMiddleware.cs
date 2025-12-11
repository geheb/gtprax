namespace GtPrax.Infrastructure.AspNetCore;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

public sealed class BotBlockerMiddleware
{
    private readonly RequestDelegate _next;

    public BotBlockerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var address = context.Connection.RemoteIpAddress?.ToString();
        if (address is null)
        {
            await _next(context);
            return;
        }

        var memoryCache = context.RequestServices.GetRequiredService<IMemoryCache>();
        if (memoryCache.TryGetValue(address, out int notFoundCounter) && notFoundCounter > 2)
        {
            context.Response.StatusCode = StatusCodes.Status418ImATeapot;
            await context.Response.WriteAsync("You are banned on this site!");
            return;
        }

        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            var expirationMinutes = new Random().Next(60, 180);
            memoryCache.Set(address, ++notFoundCounter, DateTimeOffset.UtcNow.AddMinutes(expirationMinutes));
        }
    }
}
