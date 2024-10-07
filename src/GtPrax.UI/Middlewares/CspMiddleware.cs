namespace GtPrax.UI.Middlewares;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text;

internal sealed class CspMiddleware
{
    private readonly RequestDelegate _next;
    private readonly StringValues _headerValues;

    public CspMiddleware(RequestDelegate next)
    {
        _next = next;
        _headerValues = GetHeaderValues();
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.ContentSecurityPolicy = _headerValues;

        await _next(context);
    }

    private static string GetHeaderValues()
    {
        var value = new StringBuilder();
        value.Append(GetDirective("default-src", "'self'"));
        value.Append(GetDirective("script-src", "'self'", "'unsafe-inline'"));
        value.Append(GetDirective("style-src", "'self'", "'unsafe-inline'"));
        value.Append(GetDirective("img-src", "'self'", "data:"));
        value.Append(GetDirective("font-src", "'self'"));
        value.Append(GetDirective("media-src", "'self'"));
        return value.ToString();
    }

    private static string GetDirective(string directive, params string[] sources)
        => $"{directive} {string.Join(" ", sources)}; ";
}
