using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace NokPizza.Server.Database;

public class NokPizzaDbContextFactory : IDesignTimeDbContextFactory<NokPizzaDbContext>
{
    public NokPizzaDbContext CreateDbContext(string[] args)
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environments.Development;
        var basePath = ResolveBasePath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets(typeof(NokPizzaDbContextFactory).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder()
            .UseSqlServer(
                DatabaseConnectionStringResolver.GetRequiredConnectionString(configuration)
            )
            .Options;

        return new NokPizzaDbContext(options);
    }

    private static string ResolveBasePath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directPath = Path.Combine(currentDirectory, "appsettings.json");
        if (File.Exists(directPath))
        {
            return currentDirectory;
        }

        var projectPath = Path.Combine(
            currentDirectory,
            "source",
            "NokPizza.Server",
            "appsettings.json"
        );
        if (File.Exists(projectPath))
        {
            return Path.GetDirectoryName(projectPath)
                ?? throw new InvalidOperationException(
                    "Could not resolve the NokPizza.Server project directory."
                );
        }

        throw new InvalidOperationException(
            "Could not locate appsettings.json for NokPizza.Server. Run EF commands from the solution or project directory."
        );
    }
}
