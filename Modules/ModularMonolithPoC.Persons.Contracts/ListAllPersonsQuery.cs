using MediatR;

namespace ModularMonolithPoC.Persons.Contracts;

public sealed class ListAllPersonsQuery : IRequest<ICollection<PersonDto>>;
