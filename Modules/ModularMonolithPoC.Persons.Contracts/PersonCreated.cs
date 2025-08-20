using MassTransit;

namespace ModularMonolithPoC.Persons.Contracts;
public sealed record PersonCreated : CorrelatedBy<Guid>
{
	public Guid CorrelationId => PersonId;

	public required Guid PersonId { get; init; }

	public required string Name { get; init; }
}
