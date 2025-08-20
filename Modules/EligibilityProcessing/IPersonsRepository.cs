namespace ModularMonolithPoC.EligibilityProcessing;
internal interface IPersonsRetriever
{
	Task<ICollection<Person>> GetAllPersonsAsync(CancellationToken cancellationToken);
}

