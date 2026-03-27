using Microsoft.Extensions.Configuration;

namespace NokPizza.Server.Database;

public static class DatabaseConnectionStringResolver
{
    public const string ConnectionStringName = "nok-pizza-db";

    public static string GetRequiredConnectionString(IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(ConnectionStringName)
            ?? configuration[$"SQLCONNSTR_{ConnectionStringName}"];

        return !string.IsNullOrWhiteSpace(connectionString)
            ? connectionString
            : throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found. "
                    + $"Configure 'ConnectionStrings__{ConnectionStringName}' or 'SQLCONNSTR_{ConnectionStringName}'."
            );
    }
}
