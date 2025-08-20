using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModularMonolithPoC.ApiService.Contracts;

namespace ModularMonolithPoC.Persons;

internal sealed class MigrateDatabaseStartupTask : IStartupTask
{
	public async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<MigrateDatabaseStartupTask>>();
		logger.LogInformation("Migrating database for Persons Module...");

		await using var db = serviceProvider.GetRequiredService<PersonsDbContext>();
		await db.Database.MigrateAsync(cancellationToken);
	}
}
