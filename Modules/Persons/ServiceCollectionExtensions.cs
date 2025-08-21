using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularMonolithPoC.ApiService.Contracts;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.Persons;
public static class ServiceRegistrationExtensions
{
	public static WebApplicationBuilder AddPersonsModule(this WebApplicationBuilder builder)
	{
		builder.AddNpgsqlDbContext<PersonsDbContext>("postgres");

		builder.Services.AddTransient<IStartupTask, MigrateDatabaseStartupTask>();

		builder.Services.AddMassTransit(x =>
		{
			x.AddEntityFrameworkOutbox<PersonsDbContext>(o =>
			{
				o.UsePostgres();
				o.UseBusOutbox();
			});
		});

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
		personsApis.MapPut("/{personId}", UpdatePersonAsync);
		personsApis.MapDelete("/{personId}", DeletePersonAsync);

		async Task<IResult> GetPersonsAsync(PersonsDbContext db, CancellationToken cancellationToken)
		{
			var persons = await db.Persons.ToListAsync(cancellationToken);
			return TypedResults.Ok(persons);
		}

		async Task<IResult> CreatePersonAsync(Person person, PersonsDbContext personsDbContext, IPublishEndpoint publishEndpoint, CancellationToken cancellationToken)
		{
			var exists = await personsDbContext.Persons.AnyAsync(
			p => p.Id == person.Id || p.Name.ToLower() == person.Name.ToLower(),
			cancellationToken);

			if (exists)
			{
				return TypedResults.Conflict();
			}

			personsDbContext.Persons.Add(person);

			var personCreatedEvent = new PersonCreated
			{
				PersonId = person.Id,
				Name = person.Name
			};
			await publishEndpoint.Publish(personCreatedEvent, cancellationToken);

			await personsDbContext.SaveChangesAsync(cancellationToken);

			return TypedResults.Created("/persons", person);
		}

		async Task<IResult> UpdatePersonAsync(Guid personId, Person person, PersonsDbContext personsDbContext, IPublishEndpoint publishEndpoint, CancellationToken cancellationToken)
		{
			var personInDb = await personsDbContext.Persons.FindAsync([personId], cancellationToken);

			if (personInDb is null)
			{
				return TypedResults.NotFound();
			}

			personInDb.Name = person.Name;

			var personUpdatedEvent = new PersonUpdated
			{
				PersonId = personInDb.Id,
				Name = personInDb.Name
			};
			await publishEndpoint.Publish(personUpdatedEvent, cancellationToken);

			await personsDbContext.SaveChangesAsync(cancellationToken);

			return TypedResults.Ok(personInDb);
		}

		async Task<IResult> DeletePersonAsync(Guid personId, PersonsDbContext personsDbContext, IPublishEndpoint publishEndpoint, CancellationToken cancellationToken)
		{
			var person = await personsDbContext.Persons.FindAsync([personId], cancellationToken);

			if (person is null)
			{
				return TypedResults.NotFound();
			}

			personsDbContext.Persons.Remove(person);

			var personDeletedEvent = new PersonDeleted
			{
				PersonId = person.Id
			};
			await publishEndpoint.Publish(personDeletedEvent, cancellationToken);

			await personsDbContext.SaveChangesAsync(cancellationToken);

			return TypedResults.NoContent();
		}
	}

}
