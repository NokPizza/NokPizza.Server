namespace NokPizza.AppHost;

public static class ResourceBuilderExtensions
{
    public static IResourceBuilder<ProjectResource> WithApiDevDefaults(
        this IResourceBuilder<ProjectResource> builder
    )
    {
        builder.WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");
        builder.WithEnvironment("USE_ASPIRE_OTEL", "true");

        return builder;
    }
}
