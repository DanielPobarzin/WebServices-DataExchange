using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Server.Application.Interfaces.Services;
using static CSharpFunctionalExtensions.Result;

namespace Server.Infrastructure;

internal class Program
{
	private static void Main()
	{
		
		var services = new ServiceCollection();
		services.AddApplicationServices();
		using var serviceProvider = services.BuildServiceProvider();
		var configService = serviceProvider.GetService<IConfigService>();
			var config = configService.GetConfiguration();
		
		var hostService = serviceProvider.GetService<ISelfHost>();
		if (hostService is null) return;

		hostService.StartHostedServiceAsync(config);
		configService.StartAsync();

		Task.Delay(Timeout.Infinite).Wait();
	}
}
