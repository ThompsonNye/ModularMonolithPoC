using DispatchR.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularMonolithPoC.ApiService.Contracts;
using ModularMonolithPoC.Persons.Contracts;
using Wolverine;
using Wolverine.RabbitMQ;

namespace ModularMonolithPoC.Persons;
public static class DependencyInjectionExtensions
{
	public static WebApplicationBuilder AddPersonsModule(this WebApplicationBuilder builder)
	{
		builder.AddNpgsqlDbContext<PersonsDbContext>("postgres");

		builder.Services.AddDispatchR(typeof(IPersonsMarker).Assembly, withNotifications: true);

		builder.Services.AddTransient<IStartupTask, MigrateDatabaseStartupTask>();

		return builder;
	}

	public static WolverineOptions ConfigurePersonsModule(this WolverineOptions options)
	{
		options.PublishMessage<PersonCreated>().ToRabbitExchange(MessagingConstants.Exchanges.PersonCreatedExchange);
		options.PublishMessage<PersonUpdated>().ToRabbitExchange(MessagingConstants.Exchanges.PersonUpdatedExchange);
		options.PublishMessage<PersonDeleted>().ToRabbitExchange(MessagingConstants.Exchanges.PersonDeletedExchange);

		return options;
	}

	public static WebApplication UsePersonsModule(this WebApplication app)
	{
		MapEndpoints(app);
		return app;
	}

	private static void MapEndpoints(WebApplication app)
	{
		var personsApis = app.MapGroup("/persons");

		personsApis
			.MapGet("/", GetPersonsAsync)
			.WithName("Get all persons");
		personsApis
			.MapPost("/", CreatePersonAsync)
			.WithName("Create person");
		personsApis
			.MapPut("/{personId}", UpdatePersonAsync)
			.WithName("Update person");
		personsApis
			.MapDelete("/{personId}", DeletePersonAsync)
			.WithName("Delete person");

		async Task<IResult> GetPersonsAsync(PersonsDbContext db, CancellationToken cancellationToken)
		{
			var persons = await db.Persons.ToListAsync(cancellationToken);
			return TypedResults.Ok(persons);
		}

		async Task<IResult> CreatePersonAsync(Person person, PersonsDbContext personsDbContext, IMessageBus messageBus, CancellationToken cancellationToken)
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
			await personsDbContext.SaveChangesAsync(cancellationToken);

			await messageBus.PublishAsync(personCreatedEvent);

			return TypedResults.Created("/persons", person);
		}

		async Task<IResult> UpdatePersonAsync(Guid personId, Person person, PersonsDbContext personsDbContext, IMessageBus messageBus, CancellationToken cancellationToken)
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
			await personsDbContext.SaveChangesAsync(cancellationToken);

			await messageBus.PublishAsync(personUpdatedEvent);

			return TypedResults.Ok(personInDb);
		}

		async Task<IResult> DeletePersonAsync(Guid personId, PersonsDbContext personsDbContext, IMessageBus messageBus, CancellationToken cancellationToken)
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
			await personsDbContext.SaveChangesAsync(cancellationToken);

			await messageBus.PublishAsync(personDeletedEvent);

			return TypedResults.NoContent();
		}
	}

}
