using MassTransit;

namespace ModularMonolithPoC.Persons.Contracts;

public sealed record PersonDeleted : CorrelatedBy<Guid>
{
	public Guid CorrelationId => PersonId;

	public required Guid PersonId { get; init; }
}
