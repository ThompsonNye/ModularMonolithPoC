namespace ModularMonolithPoC.Persons.Contracts;

public sealed record PersonDto
{
	public required Guid Id { get; init; }

	public required string Name { get; init; }
}
