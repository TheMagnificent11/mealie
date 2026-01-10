var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("mealie");

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")
    .WithPasswordAuthentication()
    .RunAsContainer(container =>
    {
        container
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume(isReadOnly: false);
    });

var mealieDb = postgres.AddDatabase("mealiedb");

#pragma warning disable CS8604 // Possible null reference argument.
builder.AddContainer("mealie-app", "ghcr.io/mealie-recipes/mealie", "v3.9.2")
    .WithHttpEndpoint(port: 80, targetPort: 9000, name: "http")
    .WithEnvironment("ALLOW_SIGNUP", "false")
    .WithEnvironment("PUID", "1000")
    .WithEnvironment("PGID", "1000")
    .WithEnvironment("TZ", "Australia/Brisbane")
    .WithEnvironment("DB_ENGINE", "postgres")
    .WithEnvironment("POSTGRES_USER", postgres.Resource.UserName)
    .WithEnvironment("POSTGRES_PASSWORD", postgres.Resource.Password)
    .WithEnvironment("POSTGRES_SERVER", postgres.Resource.Host)
    .WithEnvironment("POSTGRES_PORT", postgres.Resource.Port)
    .WithEnvironment("POSTGRES_DB", "mealiedb")
    .WithVolume("mealie-data", "/app/data")
    .WithReference(mealieDb)
    .WaitFor(mealieDb);
#pragma warning restore CS8604 // Possible null reference argument.

await builder.Build().RunAsync();
