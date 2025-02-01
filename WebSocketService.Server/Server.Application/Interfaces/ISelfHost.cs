using Server.Application.ConfigurationApp;

namespace Server.Application.Interfaces
{
	/// <summary>
	/// Служба обслуживания хоста
	/// </summary>
	public interface ISelfHost
	{
		/// <summary>
		/// Запуск хостовой службы
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task StartHostedServiceAsync(AppSetting setting, CancellationToken cancellationToken = default);

		/// <summary>
		/// Останов хостовой службы
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task StopHostedServiceAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Перезапуск хостовой службы
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task RebootHostedServiceAsync(AppSetting setting, CancellationToken cancellationToken = default);

	}
}
