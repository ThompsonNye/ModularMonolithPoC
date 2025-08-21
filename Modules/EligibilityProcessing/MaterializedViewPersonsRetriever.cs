using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed class MaterializedViewPersonsRetriever(
    MaterializedPersonsDbContext materializedPersonsDbContext,
    ActivitySource activitySource)
    : IPersonsRetriever
{
    public async Task<ICollection<Person>> GetAllPersonsAsync(CancellationToken cancellationToken)
    {
        using var activity = activitySource.StartActivity("Get all Persons - Materialized View");

		var persons = await materializedPersonsDbContext.Persons.ToListAsync(cancellationToken);
		return persons;
    }
}
