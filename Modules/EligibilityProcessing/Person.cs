namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed record Person
{
	public required Guid Id { get; init; }
	public required string Name { get; set; }
}
