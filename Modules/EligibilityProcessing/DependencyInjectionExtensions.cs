using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularMonolithPoC.ApiService.Contracts;
using Wolverine;
using Wolverine.RabbitMQ;

namespace ModularMonolithPoC.EligibilityProcessing;
public static class DependencyInjectionExtensions
{
	public static WebApplicationBuilder AddEligibilityProcessingModule(this WebApplicationBuilder builder)
	{
		builder.AddNpgsqlDbContext<MaterializedPersonsDbContext>("postgres");

		builder.Services.AddKeyedScoped<IPersonsRetriever, MediatRPersonsRetriever>(nameof(MediatRPersonsRetriever));
		builder.Services.AddKeyedScoped<IPersonsRetriever, MaterializedViewPersonsRetriever>(nameof(MaterializedViewPersonsRetriever));
		builder.Services.AddTransient<IStartupTask, MigrateDatabaseStartupTask>();

		return builder;
	}

	public static WolverineOptions ConfigureEligibilityProcessingModule(this WolverineOptions options)
	{
		options.Discovery.IncludeAssembly(typeof(IEligibilityProcessingMarker).Assembly);
		options.Discovery.CustomizeHandlerDiscovery(x => x.Includes.IsNotPublic());

		options.ListenToRabbitQueue("eligibility.person-created-queue");
		options.ListenToRabbitQueue("eligibility.person-updated-queue");
		options.ListenToRabbitQueue("eligibility.person-deleted-queue");

		return options;
	}

	public static WebApplication UseEligibilityProcessingModule(this WebApplication app)
	{
		MapEndpoints(app);
		return app;
	}

	private static void MapEndpoints(WebApplication app)
	{
		var eligibilityApis = app.MapGroup("/eligibility");

		eligibilityApis.MapGet("/all-persons", GetAllPersonsWithEligibility);

		async Task<IResult> GetAllPersonsWithEligibility([FromQuery] bool? useMediator, IServiceProvider serviceProvider, CancellationToken cancellationToken)
		{
			var personsRetrieverServiceKey = (useMediator ?? false)
				? nameof(MediatRPersonsRetriever)
				: nameof(MaterializedViewPersonsRetriever);

			var personsRetriever = serviceProvider.GetRequiredKeyedService<IPersonsRetriever>(personsRetrieverServiceKey);

			var persons = await personsRetriever.GetAllPersonsAsync(cancellationToken);

			var personsEligibility = persons
				.Select(p =>
				{
					var random = new Random(p.Id.GetHashCode());

					return new PersonEligibility
					{
						Name = p.Name,
						Score = (byte)random.Next(101)
					};
				});

			return TypedResults.Ok(personsEligibility);
		}
	}
}
