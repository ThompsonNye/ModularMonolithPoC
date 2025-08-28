using ModularMonolithPoC.Persons.Contracts;
using Wolverine;

namespace ModularMonolithPoC.EligibilityProcessing;
public sealed class PersonCreatedHandler(MaterializedPersonsDbContext materializedPersonsDbContext) : IWolverineHandler
{
	public async Task Handle(PersonCreated personCreated)
	{
		var person = new Person
		{
			Id = personCreated.PersonId,
			Name = personCreated.Name,
		};
		materializedPersonsDbContext.Persons.Add(person);
		await materializedPersonsDbContext.SaveChangesAsync();
	}
}
