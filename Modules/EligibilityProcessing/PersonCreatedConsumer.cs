using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;
internal sealed class PersonCreatedConsumer(MaterializedPersonsDbContext materializedPersonsDbContext)
{
	public async Task Consume(PersonCreated personCreated, CancellationToken cancellationToken)
	{
		var person = new Person
		{
			Id = personCreated.PersonId,
			Name = personCreated.Name,
		};
		materializedPersonsDbContext.Persons.Add(person);
		await materializedPersonsDbContext.SaveChangesAsync(cancellationToken);
	}
}
