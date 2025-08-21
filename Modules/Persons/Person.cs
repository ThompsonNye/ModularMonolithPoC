namespace ModularMonolithPoC.Persons;

internal sealed record Person
{
	public required Guid Id { get; init; }

	public required string Name { get; set; }
}
