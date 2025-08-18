using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularMonolithPoC.ApiService.Contracts;

namespace ModularMonolithPoC.Persons;
public static class ServiceRegistrationExtensions
{
	public static WebApplicationBuilder AddPersonsModule(this WebApplicationBuilder builder)
	{
		builder.AddNpgsqlDbContext<PersonsDbContext>("postgres");

		builder.Services.AddTransient<IStartupTask, MigrateDatabaseStartupTask>();

		return builder;
	}
}
