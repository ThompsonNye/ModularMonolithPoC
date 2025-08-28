using Microsoft.Extensions.Logging;
using ModularMonolithPoC.Persons.Contracts;
using Wolverine;

namespace ModularMonolithPoC.EligibilityProcessing;

public sealed class PersonDeletedHandler(
	MaterializedPersonsDbContext materializedPersonsDbContext,
	ILogger<PersonDeletedHandler> logger)
	: IWolverineHandler
{
	public async Task Handle(PersonDeleted personDeleted, CancellationToken cancellationToken)
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
