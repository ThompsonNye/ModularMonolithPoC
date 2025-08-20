namespace ModularMonolithPoC.Persons;

public sealed record Person
{
	public required Guid Id { get; init; }

	public required string Name { get; init; }
}
