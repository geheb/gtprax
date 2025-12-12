namespace GtPrax.Infrastructure.AspNetCore;

using System.Net;
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
        var address = context.Connection.RemoteIpAddress;
        if (address is null || address.Equals(IPAddress.Loopback) || address.Equals(IPAddress.IPv6Loopback))
        {
            await _next(context);
            return;
        }

        var key = address.ToString();

        var memoryCache = context.RequestServices.GetRequiredService<IMemoryCache>();
        if (memoryCache.TryGetValue(key, out int notFoundCounter) && notFoundCounter > 2)
        {
            context.Response.StatusCode = StatusCodes.Status418ImATeapot;
            await context.Response.WriteAsync("You are banned on this site!");
            return;
        }

        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            var expirationMinutes = new Random().Next(60, 180);
            memoryCache.Set(key, ++notFoundCounter, DateTimeOffset.UtcNow.AddMinutes(expirationMinutes));
        }
    }
}
