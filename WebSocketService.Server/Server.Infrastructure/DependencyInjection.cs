using Microsoft.Extensions.DependencyInjection;
using Server.Application.Services;
using Server.DataApplicationLayer;
using Server.DataApplicationLayer.Extensions;
using Server.Domain.Messages;
using Server.Infrastructure.ConfigurationApp;

namespace Server.Infrastructure
{
	/// <summary>
	/// Методы расширения для настройки служб в <see cref="IServiceCollection"/>.
	/// </summary>
	internal static class DependencyInjection
	{
		/// <summary>
		/// Регистрирует службы приложения в <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">Коллекция сервисов <see cref="IServiceCollection"/>, в которую нужно добавить службы.</param>
		/// <returns>Одна и та же коллекция сервисов, позволяющая объединить несколько вызовов.</returns>
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddUnitOfWork<ApplicationDbContext<Notification>>();
			services.AddUnitOfWork<ApplicationDbContext<Alarm>>();
			services.AddSingleton<ISelfHost, SelfHost>();
			services.AddSingleton<ILogService, LogService>();
			services.AddSingleton<IConfigService, ConfigService>();

			return services;
		}
	}
}	