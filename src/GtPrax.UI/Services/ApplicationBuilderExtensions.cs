namespace GtPrax.UI.Services;

internal static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCsp(this IApplicationBuilder app)
    {
        var builder = new CspOptionsBuilder();

        builder.Defaults.AllowSelf();
        builder.Connect.AllowSelf();
        builder.Scripts.AllowSelf().AllowUnsafeInline();
        builder.Styles.AllowSelf().AllowUnsafeInline();
        builder.Fonts.AllowSelf();
        builder.Images.AllowSelf();

        var options = builder.Build();
        return app.UseMiddleware<CspMiddleware>(options);
    }
}
