using DispatchR.Abstractions.Send;

namespace ModularMonolithPoC.Persons.Contracts;

public sealed class ListAllPersonsQuery : IRequest<ListAllPersonsQuery, Task<ICollection<PersonDto>>>;
