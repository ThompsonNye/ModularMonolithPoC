namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed record Person
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
}
