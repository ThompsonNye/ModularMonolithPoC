namespace ModularMonolithPoC.Persons.Contracts;

public sealed record PersonDeleted
{
	public required Guid PersonId { get; init; }
}
