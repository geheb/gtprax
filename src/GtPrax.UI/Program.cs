using System.Globalization;
using GtPrax.Application;
using GtPrax.Infrastructure;
using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using GtPrax.UI.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
        .CreateBootstrapLogger();

    Log.Information("Application started");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture));

    builder.WebHost.ConfigureKestrel(o =>
    {
        o.AddServerHeader = false;
        o.AllowResponseHeaderCompression = false;
    });

    var config = builder.Configuration;
    var services = builder.Services;

    services.AddAuthentication(IdentityConstants.ApplicationScheme)
        .AddCookie(IdentityConstants.ApplicationScheme, options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.Name = "__GtPrax-AppToken";
            options.ExpireTimeSpan = TimeSpan.FromHours(1);

            options.LoginPath = "/Login";
            options.LogoutPath = "/Login/Exit";
            options.AccessDeniedPath = "/Error/403";
            options.SlidingExpiration = true;
        });

    services.AddAuthorization();

    services.AddInfrastructure(config);
    services.AddApplication(config);

    services.Configure<AntiforgeryOptions>(options =>
    {
        options.Cookie.Name = "__GtPrax-XCsrfToken";
    });

    services.Configure<ForwardedHeadersOptions>(
        options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    services.AddRazorPages();
    services.Configure<PageContentOptions>(config.GetSection("PageContent"));
    services.AddSingleton<NodeGeneratorService>();

    var app = builder.Build();

    app.UseSerilogRequestLogging(o =>
    {
        o.MessageTemplate = "{RemoteIpAddress} @ {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        o.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        };
    });

    app.UseRequestLocalization("de-DE");
    app.UseCsp();

    app.UseNodeGenerator(typeof(GtPrax.UI.Pages.IndexModel).Assembly);
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapRazorPages();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.Information("Application exited");
    Log.CloseAndFlush();
}
