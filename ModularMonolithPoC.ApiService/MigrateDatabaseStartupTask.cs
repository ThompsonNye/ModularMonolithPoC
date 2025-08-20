using Microsoft.EntityFrameworkCore;
using ModularMonolithPoC.ApiService.Contracts;

namespace ModularMonolithPoC.ApiService;

internal sealed class MigrateDatabaseStartupTask : IStartupTask
{
	public async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<MigrateDatabaseStartupTask>>();
		logger.LogInformation("Migrating database for the root Api Service...");

		await using var db = serviceProvider.GetRequiredService<MasstransitDbContext>();
		await db.Database.MigrateAsync(cancellationToken);
	}
}
