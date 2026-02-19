using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace GtPrax.Infrastructure.Database;

public static class SqliteCustomFunctions
{
    private sealed class SqliteConnectionInterceptor : DbConnectionInterceptor
    {
        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData) =>
            ((SqliteConnection)connection).RegisterContains();

        public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            ((SqliteConnection)connection).RegisterContains();
            return Task.CompletedTask;
        }
    }

    // SQLite Like cannot handle Unicode, so we are building a custom function
    // https://learn.microsoft.com/de-de/ef/core/providers/sqlite/functions
    public static bool Contains(string? name, string? value) => throw new NotSupportedException();

    public static void RegisterCustomFunctions(this ModelBuilder builder)
    {
        builder.HasDbFunction(
            typeof(SqliteCustomFunctions).GetMethod(nameof(Contains), [typeof(string), typeof(string)])!,
            b =>
            {
                b.HasName("CONTAINS");
                b.HasParameter("name").PropagatesNullability();
                b.HasParameter("value").PropagatesNullability();
            });
    }

    public static void RegisterCustomFunctions(this DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.AddInterceptors(new SqliteConnectionInterceptor());

    private static void RegisterContains(this SqliteConnection connection) =>
        connection.CreateFunction(
            "contains",
            (string name, string value) =>
                !string.IsNullOrEmpty(name) && name.Contains(value, StringComparison.OrdinalIgnoreCase));

}
