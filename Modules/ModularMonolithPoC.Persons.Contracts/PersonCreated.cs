namespace ModularMonolithPoC.Persons.Contracts;

public sealed record PersonCreated
{
	public required Guid PersonId { get; init; }

	public required string Name { get; init; }
}
