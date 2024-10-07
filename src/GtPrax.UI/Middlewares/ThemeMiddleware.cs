namespace GtPrax.UI.Middlewares;

using GtPrax.UI.Extensions;

public class ThemeMiddleware
{
    private readonly RequestDelegate _next;

    public ThemeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var isDark = context.Request.Cookies[CookieNames.Theme] == "0";

        if (context.Request.Query.TryGetValue("theme", out var value))
        {
            isDark = value == "0";
            context.Response.Cookies.Append(CookieNames.Theme, isDark ? "0" : "1", new CookieOptions { HttpOnly = true, MaxAge = TimeSpan.FromDays(400), SameSite = SameSiteMode.Strict });
        }

        context.Items["theme"] = isDark ? "dark" : "light";

        await _next(context);
    }
}
