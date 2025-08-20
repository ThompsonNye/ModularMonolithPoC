using MassTransit;
using Microsoft.Extensions.Logging;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed class PersonUpdatedConsumer(
	MaterializedPersonsDbContext materializedPersonsDbContext,
	ILogger<PersonUpdatedConsumer> logger)
	: IConsumer<PersonUpdated>
{
	public async Task Consume(ConsumeContext<PersonUpdated> context)
	{
		var person = await materializedPersonsDbContext.Persons.FindAsync([context.Message.PersonId], context.CancellationToken);

		if (person is null)
		{
			logger.LogWarning("Received event '{EventName}', but no such person was found locally. Inserting new person", nameof(PersonUpdated));

			person = new Person
			{
				Id = context.Message.PersonId,
				Name = context.Message.Name,
			};

			materializedPersonsDbContext.Persons.Add(person);
			await materializedPersonsDbContext.SaveChangesAsync(context.CancellationToken);
			return;
		}

		person.Name = context.Message.Name;
		await materializedPersonsDbContext.SaveChangesAsync(context.CancellationToken);
	}
}
