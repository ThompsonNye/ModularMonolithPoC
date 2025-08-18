using ModularMonolithPoC.ApiService.Contracts;

internal sealed class StartupTaskRunner(
	IServiceProvider serviceProvider,
	IEnumerable<IStartupTask> startupTasks,
	ILogger<StartupTaskRunner> logger)
	: IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var tasks = startupTasks.ToList();
		logger.LogInformation("Running {Count} startup task(s)...", tasks.Count);

		foreach (var task in tasks)
		{
			logger.LogDebug("Running startup task: {StartupTaskName}", task.GetType().Name);

			using var scope = serviceProvider.CreateScope();
			await task.RunAsync(scope.ServiceProvider, cancellationToken);
		}

		logger.LogInformation("Done running startup tasks");
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
