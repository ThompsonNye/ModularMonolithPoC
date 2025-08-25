using DispatchR.Abstractions.Send;
using Microsoft.EntityFrameworkCore;
using ModularMonolithPoC.Persons.Contracts;
using System.Diagnostics;

namespace ModularMonolithPoC.Persons;

internal class ListAllPersonsViaDispatchRQueryHandler(
	PersonsDbContext personsDbContext,
	ActivitySource activitySource)
	: IRequestHandler<ListAllPersonsViaDispatchRQuery, Task<ICollection<PersonDto>>>
{
	public async Task<ICollection<PersonDto>> Handle(ListAllPersonsViaDispatchRQuery request, CancellationToken cancellationToken)
	{
		using var _ = activitySource.StartActivity(nameof(ListAllPersonsViaDispatchRQueryHandler));

		var persons = await personsDbContext.Persons
			.Select(p => new PersonDto
			{
				Id = p.Id,
				Name = p.Name,
			})
			.ToListAsync(cancellationToken);

		return persons;
	}
}
