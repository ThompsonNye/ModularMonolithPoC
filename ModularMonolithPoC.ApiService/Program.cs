using MassTransit;
using ModularMonolithPoC.ApiService;
using ModularMonolithPoC.ApiService.Contracts;
using ModularMonolithPoC.EligibilityProcessing;
using ModularMonolithPoC.Persons;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.AddPersonsModule();
builder.AddEligibilityProcessingModule();

builder.Services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssemblyContaining<IPersonsMarker>();
});

builder.Services.AddMassTransit(x =>
{
	x.SetKebabCaseEndpointNameFormatter();

	x.AddConsumersFromAssemblyContaining<IEligibilityProcessingMarker>();

	x.UsingRabbitMq((context, cfg) =>
	{
		var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq");
		var rabbitMqUri = new Uri(rabbitMqConnectionString!);

		var userInfo = rabbitMqUri.UserInfo;
		var firstColonIndex = userInfo.IndexOf(':');
		if (firstColonIndex == -1)
		{
			throw new ArgumentException("Could not identify username and password from rabbit mq connection string");
		}

		var username = userInfo[..firstColonIndex];
		var password = userInfo[(firstColonIndex + 1)..];

		cfg.Host(
			new Uri($"{rabbitMqUri.Scheme}://{rabbitMqUri.Host}:{rabbitMqUri.Port}"),
			"/",
			hostConfig =>
			{
				hostConfig.Username(username);
				hostConfig.Password(password);
			});

		cfg.UseDelayedRedelivery(r =>
		{
			r.Intervals(
				TimeSpan.FromMinutes(5),
				TimeSpan.FromMinutes(15),
				TimeSpan.FromMinutes(30));
			r.Ignore<ValidationException>();
		});
		cfg.UseMessageRetry(r =>
		{
			r.Incremental(
				5,
				TimeSpan.Zero,
				TimeSpan.FromSeconds(5));
			r.Ignore<ValidationException>();
		});

		cfg.ConfigureEndpoints(context);
	});
});
builder.Services.AddOpenTelemetry()
	.WithMetrics(b => b.AddMeter(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName))
	.WithTracing(o => o
		.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName));

builder.Services.AddHostedService<StartupTaskRunner>();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.UsePersonsModule();
app.UseEligibilityProcessingModule();

app.Run();
