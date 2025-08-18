namespace ModularMonolithPoC.ApiService.Contracts;

public interface IStartupTask
{
	Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
