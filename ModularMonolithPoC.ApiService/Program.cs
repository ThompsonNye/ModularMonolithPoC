using ModularMonolithPoC.EligibilityProcessing;
using ModularMonolithPoC.Persons;
using ModularMonolithPoC.PersonsAccessorWithDispatchR;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.AddPersonsModule();
builder.AddEligibilityProcessingModule();
builder.AddPersonsAccessorWithDispatchRModule();

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<IPersonsMarker>(); });

builder.UseWolverine(options =>
{
    options
        .UseRabbitMqUsingNamedConnection("rabbitmq")
        .AutoProvision();

    options
        .ConfigurePersonsModule()
        .ConfigureEligibilityProcessingModule();
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(b => b.AddMeter("Wolverine"))
    .WithTracing(o => o
        .AddSource("Wolverine"));

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
app.UsePersonsAccessorWithDispatchRModule();

app.Run();