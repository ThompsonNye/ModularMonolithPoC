using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;
public static class ServiceRegistrationExtensions
{
	public static WebApplicationBuilder AddEligibilityProcessingModule(this WebApplicationBuilder builder)
	{
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

		async Task<IResult> GetAllPersonsWithEligibility(IMediator mediator, CancellationToken cancellationToken)
		{
			var persons = await mediator.Send(new ListAllPersonsQuery(), cancellationToken);

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
