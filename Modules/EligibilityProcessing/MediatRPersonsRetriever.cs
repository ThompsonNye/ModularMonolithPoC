using MediatR;
using ModularMonolithPoC.Persons.Contracts;
using System.Diagnostics;

namespace ModularMonolithPoC.EligibilityProcessing;

public sealed class MediatRPersonsRetriever(
	ISender sender,
	ActivitySource activitySource)
	: IPersonsRetriever
{
	public async Task<ICollection<Person>> GetAllPersonsAsync(CancellationToken cancellationToken)
	{
		using var activity = activitySource.StartActivity();

		ICollection<PersonDto> personDtos;
		using (var getPersonsViaMediatRActivity = activitySource.StartActivity($"Query MediatR: {nameof(ListAllPersonsQuery)}"))
		{
			personDtos = await sender.Send(new ListAllPersonsQuery(), cancellationToken);
		}

		var persons = personDtos.Select(p => new Person
		{
			Id = p.Id,
			Name = p.Name,
		}).ToList();
		return persons;
	}
}

