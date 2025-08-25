using DispatchR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.PersonsAccessorWithDispatchR;

public static class DependencyInjectionExtensions
{
	public static WebApplicationBuilder AddPersonsAccessorWithDispatchRModule(this WebApplicationBuilder builder)
	{
		return builder;
	}

	public static WebApplication UsePersonsAccessorWithDispatchRModule(this WebApplication app)
	{
		var group = app.MapGroup("/persons-from-dispatchr");
		group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
		{
			var persons = await mediator.Send(new ListAllPersonsViaDispatchRQuery(), cancellationToken);
			return TypedResults.Ok(persons);
		});

		return app;
	}
}
