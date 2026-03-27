using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;

namespace NokPizza.AppHost.Tests;

public class AppHostResourceTests
{
    [Fact]
    public async Task AppHostBuildsSuccessfully()
    {
        using var app = await BuildAppAsync();

        app.ShouldNotBeNull();
    }

    [Fact]
    public async Task AppHostContainsSqlServerResource()
    {
        using var app = await BuildAppAsync();

        var sqlServer = app
            .Services.GetRequiredService<DistributedApplicationModel>()
            .Resources.SingleOrDefault(resource => resource.Name == "nok-pizza-db");

        sqlServer.ShouldNotBeNull();
        sqlServer.ShouldBeAssignableTo<IResourceWithConnectionString>();
    }

    [Fact]
    public async Task AppHostContainsServerResource()
    {
        using var app = await BuildAppAsync();

        var server = app
            .Services.GetRequiredService<DistributedApplicationModel>()
            .Resources.SingleOrDefault(resource => resource.Name == "nok-pizza-server");

        server.ShouldNotBeNull();
        server.ShouldBeAssignableTo<ProjectResource>();
    }

    [Fact]
    public async Task ServerResourceReferencesSqlServer()
    {
        using var app = await BuildAppAsync();

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();
        var server = model.Resources.Single(resource => resource.Name == "nok-pizza-server");

        server.Annotations.OfType<EnvironmentCallbackAnnotation>().ShouldNotBeEmpty();
    }

    [Fact]
    public async Task AppHostHasExpectedResourceCount()
    {
        using var app = await BuildAppAsync();

        var resources = app.Services.GetRequiredService<DistributedApplicationModel>().Resources;

        // sql server, sql server volume, server project, and DbPassword parameter
        resources.Count.ShouldBe(4);
    }

    private static async Task<IHost> BuildAppAsync()
    {
        var cancellationToken = CancellationToken.None;

        var appHost =
            await DistributedApplicationTestingBuilder.CreateAsync<Projects.NokPizza_AppHost>(
                cancellationToken
            );

        return await appHost.BuildAsync(cancellationToken);
    }
}
