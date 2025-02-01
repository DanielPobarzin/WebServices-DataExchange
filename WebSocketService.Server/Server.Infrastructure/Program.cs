using Microsoft.Extensions.DependencyInjection;
using Server.Application.Interfaces;
using Server.Application.Services;
using Server.DataApplicationLayer;
using Server.DataApplicationLayer.UnitOfWork;
using Server.Domain.Messages;

namespace Server.Infrastructure;

internal class Program
{
	private IServiceCollection services;
	private static async Task Main()
	{
		
		var services = new ServiceCollection();
		services.AddApplicationServices();
		await using var serviceProvider = services.BuildServiceProvider();
		var configService = serviceProvider.GetService<IConfigService>();
		if (configService is null) return;
		var config = configService.GetConfiguration();

		configService.StartAsync();
		var hostService = serviceProvider.GetService<ISelfHost>();
		if (hostService is null) return;

		hostService.StartHostedServiceAsync(config);

		var uow = serviceProvider.GetService<ILoadDataService<Alarm>>();
		var data = uow.GetAllData();
		foreach (var d in data)
		{
			Console.WriteLine(d.Content);
		}

		var channelName = "core_db_event";
		var cts = new CancellationTokenSource();
		var listeningTask = uow.ListenNotification(channelName);
		await using var dbContext = new ApplicationDbContext<Alarm>(configService);
		await dbContext.Database.EnsureCreatedAsync();

		cts.Cancel();
		await listeningTask;

		Task.Delay(Timeout.Infinite).Wait();
	}


}