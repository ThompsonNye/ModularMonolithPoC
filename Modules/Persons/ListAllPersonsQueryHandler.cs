using MediatR;
using Microsoft.EntityFrameworkCore;
using ModularMonolithPoC.Persons.Contracts;

namespace ModularMonolithPoC.Persons;
internal class ListAllPersonsQueryHandler(
	PersonsDbContext personsDbContext)
	: IRequestHandler<ListAllPersonsQuery, ICollection<PersonDto>>
{
	public async Task<ICollection<PersonDto>> Handle(ListAllPersonsQuery request, CancellationToken cancellationToken)
	{
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
