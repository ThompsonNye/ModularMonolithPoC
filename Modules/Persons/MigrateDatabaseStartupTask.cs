using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModularMonolithPoC.ApiService.Contracts;

namespace ModularMonolithPoC.Persons;

internal sealed class MigrateDatabaseStartupTask : IStartupTask
{
	public async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
	{
		var db = serviceProvider.GetRequiredService<PersonsDbContext>();
		await db.Database.MigrateAsync(cancellationToken);
	}
}
