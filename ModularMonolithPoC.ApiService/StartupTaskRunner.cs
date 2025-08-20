using ModularMonolithPoC.ApiService.Contracts;
using System.Diagnostics;

internal sealed class StartupTaskRunner(
	IServiceProvider serviceProvider,
	IEnumerable<IStartupTask> startupTasks,
	ILogger<StartupTaskRunner> logger,
	ActivitySource activitySource)
	: IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using var startupTasksActivity = activitySource.StartActivity("Run startup tasks");

		var tasks = startupTasks.ToList();

		foreach (var (task, i) in tasks.Select((task, i) => (task, i)))
		{
			var taskName = task.GetType().Name;
			using var tasksActivity = activitySource.StartActivity(taskName);

			logger.LogInformation("Running startup task ({TaskNumber}/{TotalTaskCount}): {StartupTaskName}", i + 1, tasks.Count, taskName);

			using var scope = serviceProvider.CreateScope();
			await task.RunAsync(scope.ServiceProvider, cancellationToken);
		}

		logger.LogInformation("Done running startup tasks");
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
