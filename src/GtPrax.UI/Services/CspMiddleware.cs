namespace GtPrax.UI.Services;

using Microsoft.AspNetCore.Http;
using System.Text;

internal sealed class CspMiddleware
{
    private const string Header = "Content-Security-Policy";
    private readonly RequestDelegate _next;
    private readonly CspOptions _options;

    public CspMiddleware(RequestDelegate next, CspOptions options)
    {
        _next = next;
        _options = options;
    }

    public async Task Invoke(HttpContext context)
    {
        var header = GetHeaderValue();
        if (!string.IsNullOrEmpty(header))
        {
            context.Response.Headers[Header] = GetHeaderValue();
        }
        await _next(context);
    }

    private string GetHeaderValue()
    {
        var value = new StringBuilder();
        value.Append(GetDirective("default-src", _options.Defaults));
        value.Append(GetDirective("script-src", _options.Scripts));
        value.Append(GetDirective("style-src", _options.Styles));
        value.Append(GetDirective("img-src", _options.Images));
        value.Append(GetDirective("font-src", _options.Fonts));
        value.Append(GetDirective("media-src", _options.Media));
        value.Append(GetDirective("frame-src", _options.Frame));
        value.Append(GetDirective("connect-src", _options.Connect));
        value.Append(GetDirective("worker-src", _options.Worker));
        return value.ToString();
    }

    private static string GetDirective(string directive, ICollection<string> sources)
        => sources.Count > 0 ? $"{directive} {string.Join(" ", sources)}; " : string.Empty;
}
