var builder = DistributedApplication.CreateBuilder(args);

var superUserSecret = builder.AddParameter("SuperUser", true);

var sqliteDir = new DirectoryInfo("/data/gtprax/sqlite");
if (!sqliteDir.Exists)
{
    sqliteDir.Create();
}

var mailpitDir = new DirectoryInfo("/data/gtprax/mailpit");
if (!mailpitDir.Exists)
{
    mailpitDir.Create();
}

const string databaseFileName = "gtprax.sqlite";

var sqlite = builder.AddSqlite("Sqlite", sqliteDir.FullName, databaseFileName)
    .WithSqliteWeb(c =>
    {
        c.WithArgs(databaseFileName);
    });

var mailpit = builder.AddMailPit("mailpit")
    .WithDataBindMount(mailpitDir.FullName);

builder.AddProject<Projects.GtPrax_WebApp>("webapp")
    .WithReference(sqlite)
    .WithReference(mailpit)
    .WithHttpHealthCheck("/healthz")
    .WithEnvironment(c =>
    {
        var endpoint = mailpit.GetEndpoint("smtp");
        c.EnvironmentVariables["SMTP__SERVER"] = endpoint.Host;
        c.EnvironmentVariables["SMTP__PORT"] = endpoint.Port;

        c.EnvironmentVariables["BOOTSTRAP__SUPERUSER__PASSWORD"] = superUserSecret;
    });

using var app = builder.Build();

await app.RunAsync();
