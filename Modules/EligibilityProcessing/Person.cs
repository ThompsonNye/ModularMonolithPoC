namespace ModularMonolithPoC.EligibilityProcessing;

public sealed record Person
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
}
