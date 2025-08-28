namespace ModularMonolithPoC.Persons.Contracts;

public sealed record PersonUpdated
{
	public required Guid PersonId { get; init; }

	public required string Name { get; init; }
}
