namespace ModularMonolithPoC.EligibilityProcessing;
public interface IPersonsRetriever
{
	Task<ICollection<Person>> GetAllPersonsAsync(CancellationToken cancellationToken);
}

