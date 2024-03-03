using GtPrax.UI.Extensions;
using GtPrax.UI.Models;
using GtPrax.UI.Services;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

services.AddRazorPages();
services.Configure<AppSettings>(config.GetSection("App"));
services.Configure<PageContentSettings>(config.GetSection("PageContent"));
services.AddSingleton<NodeGenerator>();

var app = builder.Build();

app.UseNodeGenerator(typeof(GtPrax.UI.Pages.IndexModel).Assembly);
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

await app.RunAsync();
