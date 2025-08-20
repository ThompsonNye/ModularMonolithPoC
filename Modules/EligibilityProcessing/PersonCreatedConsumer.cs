using MassTransit;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;
public sealed class PersonCreatedConsumer(MaterializedPersonsDbContext materializedPersonsDbContext)
	: IConsumer<PersonCreated>
{
	public async Task Consume(ConsumeContext<PersonCreated> context)
	{
		var person = new Person
		{
			Id = context.Message.PersonId,
			Name = context.Message.Name,
		};
		materializedPersonsDbContext.Persons.Add(person);
		await materializedPersonsDbContext.SaveChangesAsync(context.CancellationToken);
	}
}
