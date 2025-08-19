using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularMonolithPoC.ApiService.Contracts;

namespace ModularMonolithPoC.Persons;
public static class ServiceRegistrationExtensions
{
	public static WebApplicationBuilder AddPersonsModule(this WebApplicationBuilder builder)
	{
		builder.AddNpgsqlDbContext<PersonsDbContext>("postgres");

		builder.Services.AddTransient<IStartupTask, MigrateDatabaseStartupTask>();

		return builder;
	}

	public static WebApplication UsePersonsModule(this WebApplication app)
	{
		MapEndpoints(app);
		return app;
	}

	private static void MapEndpoints(WebApplication app)
	{
		var personsApis = app.MapGroup("/persons");

		personsApis.MapGet("/", GetPersonsAsync);
		personsApis.MapPost("/", CreatePersonAsync);
		personsApis.MapDelete("/{personId}", DeletePersonAsync);

		async Task<IResult> GetPersonsAsync(PersonsDbContext db, CancellationToken cancellationToken)
		{
			var persons = await db.Persons.ToListAsync(cancellationToken);
			return TypedResults.Ok(persons);
		}

		async Task<IResult> CreatePersonAsync(Person person, PersonsDbContext personsDbContext, CancellationToken cancellationToken)
		{
			var exists = await personsDbContext.Persons.AnyAsync(
			p => p.Id == person.Id || p.Name.ToLower() == person.Name.ToLower(),
			cancellationToken);

			if (exists)
			{
				return TypedResults.Conflict();
			}

			personsDbContext.Persons.Add(person);
			await personsDbContext.SaveChangesAsync(cancellationToken);

			return TypedResults.Created("/persons", person);
		}

		async Task<IResult> DeletePersonAsync(Guid personId, PersonsDbContext personsDbContext, CancellationToken cancellationToken)
		{
			var person = await personsDbContext.Persons.FindAsync([personId], cancellationToken);

			if (person is null)
			{
				return TypedResults.NotFound();
			}

			personsDbContext.Persons.Remove(person);
			await personsDbContext.SaveChangesAsync(cancellationToken);

			return TypedResults.NoContent();
		}
	}

}
