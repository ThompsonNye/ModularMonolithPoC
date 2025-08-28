using Microsoft.Extensions.Logging;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed class PersonUpdatedConsumer(
	MaterializedPersonsDbContext materializedPersonsDbContext,
	ILogger<PersonUpdatedConsumer> logger)
{
	public async Task Consume(PersonUpdated personUpdated, CancellationToken cancellationToken)
	{
		var person = await materializedPersonsDbContext.Persons.FindAsync([personUpdated.PersonId], cancellationToken);

		if (person is null)
		{
			logger.LogWarning("Received event '{EventName}', but no such person was found locally. Inserting new person", nameof(PersonUpdated));

			person = new Person
			{
				Id = personUpdated.PersonId,
				Name = personUpdated.Name,
			};

			materializedPersonsDbContext.Persons.Add(person);
			await materializedPersonsDbContext.SaveChangesAsync(cancellationToken);
			return;
		}

		person.Name = personUpdated.Name;
		await materializedPersonsDbContext.SaveChangesAsync(cancellationToken);
	}
}
