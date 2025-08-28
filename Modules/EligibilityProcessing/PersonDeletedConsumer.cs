using Microsoft.Extensions.Logging;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed class PersonDeletedConsumer(
	MaterializedPersonsDbContext materializedPersonsDbContext,
	ILogger<PersonUpdatedConsumer> logger)
{
	public async Task Consume(PersonDeleted personDeleted, CancellationToken cancellationToken)
	{
		var person = await materializedPersonsDbContext.Persons.FindAsync([personDeleted.PersonId], cancellationToken);

		if (person is null)
		{
			logger.LogWarning("Received event '{EventName}', but no such person was found locally.", nameof(PersonUpdated));
			return;
		}

		materializedPersonsDbContext.Persons.Remove(person);
		await materializedPersonsDbContext.SaveChangesAsync(cancellationToken);
	}
}
