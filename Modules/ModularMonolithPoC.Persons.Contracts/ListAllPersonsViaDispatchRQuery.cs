using DispatchR.Abstractions.Send;

namespace ModularMonolithPoC.Persons.Contracts;

public sealed class ListAllPersonsViaDispatchRQuery : IRequest<ListAllPersonsViaDispatchRQuery, Task<ICollection<PersonDto>>>;
