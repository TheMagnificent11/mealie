var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")
    .WithPasswordAuthentication()
    .RunAsContainer(container =>
    {
        container.WithImage("postgres", "17")
            .WithLifetime(ContainerLifetime.Persistent);
    });

var mealieDb = postgres.AddDatabase("mealiedb");
var pgUserName = postgres.Resource.UserName;
var pgPassword = postgres.Resource.Password;

#pragma warning disable CS8604 // Possible null reference argument.
builder.AddContainer("mealie-app", "ghcr.io/mealie-recipes/mealie", "v3.9.2")
    .WithHttpEndpoint(port: 9925, targetPort: 9000, name: "http")
    .WithEnvironment("ALLOW_SIGNUP", "false")
    .WithEnvironment("PUID", "1000")
    .WithEnvironment("PGID", "1000")
    .WithEnvironment("TZ", "Australia/Brisbane")
    .WithEnvironment("DB_ENGINE", "postgres")
    .WithEnvironment("POSTGRES_USER", pgUserName)
    .WithEnvironment("POSTGRES_PASSWORD", pgPassword)
    .WithEnvironment(context =>
    {
        var endpoint = postgres.GetEndpoint("tcp");
        context.EnvironmentVariables["POSTGRES_SERVER"] = endpoint.Property(EndpointProperty.Host);
        context.EnvironmentVariables["POSTGRES_PORT"] = endpoint.Property(EndpointProperty.Port);
    })
    .WithEnvironment("POSTGRES_DB", "mealiedb")
    .WithVolume("mealie-data", "/app/data")
    .WaitFor(mealieDb);
#pragma warning restore CS8604 // Possible null reference argument.

await builder.Build().RunAsync();
