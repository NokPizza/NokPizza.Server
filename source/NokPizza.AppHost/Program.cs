using NokPizza.AppHost;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("DbPassword", true);
var sqlServer = builder
    .AddSqlServer("nok-pizza-db", dbPassword, port: 1433)
    .WithVolume("nok-pizza-db-data", "/var/opt/mssql")
    .WithLifetime(ContainerLifetime.Persistent);

builder
    .AddProject<NokPizza_Server>("nok-pizza-server", "http")
    .WithReference(sqlServer)
    .WaitFor(sqlServer)
    .WithApiDevDefaults()
    .WithEndpoint(
        "http",
        static endpoint =>
        {
            endpoint.Port = 5100;
            endpoint.IsProxied = false;
        }
    );

builder.Build().Run();
