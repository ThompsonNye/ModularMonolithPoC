using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularMonolithPoC.ApiService.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;
public static class ServiceRegistrationExtensions
{
	public static WebApplicationBuilder AddEligibilityProcessingModule(this WebApplicationBuilder builder)
	{
		builder.AddNpgsqlDbContext<MaterializedPersonsDbContext>("postgres");

		builder.Services.AddScoped<IPersonsRetriever, MediatRPersonsRetriever>();
		builder.Services.AddTransient<IStartupTask, MigrateDatabaseStartupTask>();

		return builder;
	}

	public static WebApplication UseEligibilityProcessingModule(this WebApplication app)
	{
		MapEndpoints(app);
		return app;
	}

	private static void MapEndpoints(WebApplication app)
	{
		var eligibilityApis = app.MapGroup("/eligibility");

		eligibilityApis.MapGet("/all/mediatr", GetAllPersonsWithEligibility);

		async Task<IResult> GetAllPersonsWithEligibility(IPersonsRetriever personsRetriever, CancellationToken cancellationToken)
		{
			var persons = await personsRetriever.GetAllPersonsAsync(cancellationToken);

			var random = new Random(42);

			var personsEligibility = persons
				.Select(p => new PersonEligibility
				{
					Name = p.Name,
					Score = (byte)random.Next(101)
				});

			return TypedResults.Ok(personsEligibility);
		}
	}
}
