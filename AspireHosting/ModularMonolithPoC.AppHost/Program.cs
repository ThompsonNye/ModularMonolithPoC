var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("database")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithPgWeb()
	.AddDatabase("postgres");

var rabbitMq = builder.AddRabbitMQ("rabbitmq")
	.WithDataVolume()
	.WithLifetime(ContainerLifetime.Persistent)
	.WithManagementPlugin()
	.WithOtlpExporter();

var apiService = builder.AddProject<Projects.ModularMonolithPoC_ApiService>("apiservice")
	.WithReference(postgres)
	.WaitFor(postgres)
	.WithReference(rabbitMq)
	.WaitFor(rabbitMq);

builder.AddProject<Projects.ModularMonolithPoC_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(cache)
	.WaitFor(cache)
	.WithReference(apiService)
	.WaitFor(apiService);

builder.Build().Run();
