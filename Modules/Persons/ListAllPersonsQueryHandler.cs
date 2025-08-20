using MediatR;
using Microsoft.EntityFrameworkCore;
using ModularMonolithPoC.Persons.Contracts;
using System.Diagnostics;

namespace ModularMonolithPoC.Persons;
internal class ListAllPersonsQueryHandler(
	PersonsDbContext personsDbContext,
	ActivitySource activitySource)
	: IRequestHandler<ListAllPersonsQuery, ICollection<PersonDto>>
{
	public async Task<ICollection<PersonDto>> Handle(ListAllPersonsQuery request, CancellationToken cancellationToken)
	{
		using var _ = activitySource.StartActivity(nameof(ListAllPersonsQueryHandler));

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
