using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace GtPrax.Infrastructure.Database;

internal sealed class SqliteConnectionInterceptor : DbConnectionInterceptor
{
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData) =>
        CreateCollation(connection);

    public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        CreateCollation(connection);
        return Task.CompletedTask;
    }

    // https://learn.microsoft.com/de-de/ef/core/providers/sqlite/functions
    private void CreateCollation(DbConnection connection) =>
        ((SqliteConnection)connection).CreateFunction(
            "regexp",
            (string pattern, string input) => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
}
