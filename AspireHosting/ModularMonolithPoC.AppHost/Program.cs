var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("database")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithPgWeb()
	.AddDatabase("postgres");

var apiService = builder.AddProject<Projects.ModularMonolithPoC_ApiService>("apiservice")
	.WithReference(postgres)
	.WaitFor(postgres);

builder.AddProject<Projects.ModularMonolithPoC_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(cache)
	.WaitFor(cache)
	.WithReference(apiService)
	.WaitFor(apiService);

builder.Build().Run();
