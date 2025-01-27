using Microsoft.Extensions.DependencyInjection;
using Server.Application.Services;
using Server.Infrastructure.ConfigurationApp;

namespace Server.Infrastructure;

internal class Program
{
	private static void Main()
	{
		
		var services = new ServiceCollection();
		services.AddApplicationServices();
		using var serviceProvider = services.BuildServiceProvider();
		var configService = serviceProvider.GetService<IConfigService>();
		if (configService is null) return;
		var config = configService.GetConfiguration();

		configService.StartAsync();
		var hostService = serviceProvider.GetService<ISelfHost>();
		if (hostService is null) return;

		hostService.StartHostedServiceAsync(config);

		Task.Delay(Timeout.Infinite).Wait();
	}

}
