using DispatchR;
using ModularMonolithPoC.Persons.Contracts;
using System.Diagnostics;

namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed class DispatchRPersonsRetriever(
	IMediator mediator,
	ActivitySource activitySource)
	: IPersonsRetriever
{
	public async Task<ICollection<Person>> GetAllPersonsAsync(CancellationToken cancellationToken)
	{
		using var activity = activitySource.StartActivity("Get all Persons - Mediator pattern");

		ICollection<PersonDto> personDtos;
		using (var getPersonsViaDispatchRActivity = activitySource.StartActivity($"Query DispatchR: {nameof(ListAllPersonsQuery)}"))
		{
			personDtos = await mediator.Send(new ListAllPersonsQuery(), cancellationToken);
		}

		var persons = personDtos.Select(p => new Person
		{
			Id = p.Id,
			Name = p.Name,
		}).ToList();
		return persons;
	}
}
