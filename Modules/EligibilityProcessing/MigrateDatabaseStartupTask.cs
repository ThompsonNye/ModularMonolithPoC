using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModularMonolithPoC.ApiService.Contracts;

namespace ModularMonolithPoC.EligibilityProcessing;

internal sealed class MigrateDatabaseStartupTask : IStartupTask
{
	public async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<MigrateDatabaseStartupTask>>();
		logger.LogInformation("Migrating database for Eligibility Module...");

		await using var db = serviceProvider.GetRequiredService<MaterializedPersonsDbContext>();
		await db.Database.MigrateAsync(cancellationToken);
	}
}
