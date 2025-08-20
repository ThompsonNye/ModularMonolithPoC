using MassTransit;
using MassTransit.Internals;

namespace ModularMonolithPoC.ApiService;

public static class MassTransitRegistrationExtensions
{
	public static IBusRegistrationConfigurator AddConsumersFromAssemblyContaining<TTypeInAssembly>(this IBusRegistrationConfigurator busRegistrationConfigurator)
	{
		var assembly = typeof(TTypeInAssembly).Assembly;

		var consumerImplementationsTypes = assembly.DefinedTypes
			.Where(type => type.IsInterface == false && type.IsAbstract == false && type.HasInterface(typeof(IConsumer<>)))
			.ToArray();

		busRegistrationConfigurator.AddConsumers(consumerImplementationsTypes);

		return busRegistrationConfigurator;
	}
}
