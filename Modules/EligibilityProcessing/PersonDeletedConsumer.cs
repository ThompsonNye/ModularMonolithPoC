using MassTransit;
using Microsoft.Extensions.Logging;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;

public sealed class PersonDeletedConsumer(
	MaterializedPersonsDbContext materializedPersonsDbContext,
	ILogger<PersonUpdatedConsumer> logger)
	: IConsumer<PersonDeleted>
{
	public async Task Consume(ConsumeContext<PersonDeleted> context)
	{
		var person = await materializedPersonsDbContext.Persons.FindAsync([context.Message.PersonId], context.CancellationToken);

		if (person is null)
		{
			logger.LogWarning("Received event '{EventName}', but no such person was found locally.", nameof(PersonUpdated));
			return;
		}

		materializedPersonsDbContext.Persons.Remove(person);
		await materializedPersonsDbContext.SaveChangesAsync(context.CancellationToken);
	}
}
