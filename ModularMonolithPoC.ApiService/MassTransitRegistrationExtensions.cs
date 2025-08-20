using MassTransit;

namespace ModularMonolithPoC.ApiService;

public static class MassTransitRegistrationExtensions
{
	public static IBusRegistrationConfigurator AddConsumersFromAssemblyContaining<TTypeInAssembly>(this IBusRegistrationConfigurator busRegistrationConfigurator)
	{
		var assembly = typeof(TTypeInAssembly).Assembly;
		var consumerInterfaceType = typeof(IConsumer);

		var consumerImplementationsTypes = assembly.DefinedTypes
			.Where(type => type.IsInterface == false && type.IsAbstract == false && type.IsAssignableTo(consumerInterfaceType));

		busRegistrationConfigurator.AddConsumers(consumerInterfaceType);

		return busRegistrationConfigurator;
	}
}
