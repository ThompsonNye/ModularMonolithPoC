using Microsoft.Extensions.Logging;
using ModularMonolithPoC.Persons.Contracts;
using Wolverine;

namespace ModularMonolithPoC.EligibilityProcessing;

public sealed class PersonUpdatedHandler(
	MaterializedPersonsDbContext materializedPersonsDbContext,
	ILogger<PersonUpdatedHandler> logger)
	: IWolverineHandler
{
	public async Task Handle(PersonUpdated personUpdated, CancellationToken cancellationToken)
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
