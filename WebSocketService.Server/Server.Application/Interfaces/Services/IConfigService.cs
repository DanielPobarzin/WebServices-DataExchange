using Microsoft.Extensions.Configuration;

namespace Server.Application.Interfaces.Services
{
	/// <summary>
	/// Служба предоставления конфигурации приложения.
	/// </summary>
	public interface IConfigService
	{
		/// <summary>
		///  Валидация конфигурации 
		/// </summary>
		/// <returns></returns>
		Task Validation();

		/// <summary>
		/// Запросить у службы текущую конфигурацию приложения.
		/// </summary>
		/// <returns></returns>
		IConfiguration GetConfiguration();
		/// <summary>
		/// Запуск фоновой службы отслеживания изменения конфигурации.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task StartAsync(CancellationToken cancellationToken = default);

	}
}
