using GtPrax.Application.Models;
using GtPrax.Infrastructure;
using GtPrax.Infrastructure.AspNetCore;
using GtPrax.Infrastructure.Email;
using GtPrax.Infrastructure.Extensions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Serilog;

void ConfigureApp(WebApplicationBuilder builder)
{
    builder.WebHost.ConfigureKestrel(o =>
    {
        o.AddServerHeader = false;
        o.AllowResponseHeaderCompression = false;
    });

    var config = builder.Configuration;
    var services = builder.Services;

    services.AddSerilog();
    services.AddMemoryCache();
    services.AddHttpContextAccessor();

    services.RegisterCore(config);

    if (builder.Environment.IsDevelopment())
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
    }

    services.AddAuthentication();
    services.AddAuthorization();

    services.AddControllers();
    services.AddRazorPages()
        .AddMvcOptions(options =>
        {
            options.ModelBindingMessageProvider.SetLocale();
            options.Filters.Add<OperationCancelledExceptionFilter>();
        });

    services.AddDataProtection()
        .AddCertificate("GtPrax", config.GetSection("DataProtection"));

    services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = PasswordLengthFieldAttribute.MinLen;
        options.Password.RequiredUniqueChars = 5;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(3);
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.AllowedForNewUsers = true;

        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
        options.User.RequireUniqueEmail = true;
    });

    services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        // lifespan of issued tokens (changing email or password)
        options.TokenLifespan = TimeSpan.FromDays(1);
    });

    services.Configure<SecurityStampValidatorOptions>(options =>
    {
        // A user accessing the site with an existing cookie would be validated, and a new cookie would be issued. 
        // This process is completely silent and happens behind the scenes.
        options.ValidationInterval = TimeSpan.FromMinutes(15);
    });

    services.ConfigureApplicationCookie(options =>
    {
        // Cookie settings
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "__GtPrax-WebApp";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);

        options.LoginPath = "/Login";
        options.LogoutPath = "/Login/Exit";
        options.AccessDeniedPath = "/Error/403";
        options.SlidingExpiration = true;
    });

    services.Configure<AntiforgeryOptions>(options =>
    {
        options.Cookie.Name = "__GtPrax-XCsrfToken";
        options.HeaderName = "X-CSRF-TOKEN";
        options.FormFieldName = "__XCsrfToken";
    });

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });

    services.Configure<AppSettings>(config.GetSection("App"));
    services.Configure<SmtpSettings>(config.GetSection("Smtp"));
    services.Configure<PageContentSettings>(config.GetSection("PageContent"));
}

void ConfigurePipeline(WebApplication app)
{
    app.UseForwardedHeaders();

    app.UseRequestLocalization("de-DE");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseMiddleware<BotBlockerMiddleware>();
    app.UseMiddleware<CspMiddleware>();

    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapRazorPages();
    app.MapControllers();
    app.UseNodeGenerator();
}

try
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("logging.json")
        .Build();

    var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

    Log.Logger = logger;

    Log.Information("Application started");

    var builder = WebApplication.CreateBuilder(args);

    ConfigureApp(builder);

    using var app = builder.Build();
    ConfigurePipeline(app);

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
    await Log.CloseAndFlushAsync();
}
