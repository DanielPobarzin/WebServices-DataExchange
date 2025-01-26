using Microsoft.Extensions.Configuration;

namespace Server.Application.Interfaces.Services
{
	/// <summary>
	/// Служба обслуживания хоста
	/// </summary>
	public interface ISelfHost
	{
		/// <summary>
		/// Запуск хостовой службы
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task StartHostedServiceAsync(IConfiguration configuration, CancellationToken cancellationToken = default);

		/// <summary>
		/// Останов хостовой службы
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task StopHostedServiceAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Перезапуск хостовой службы
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task RebootHostedServiceAsync(IConfiguration configuration, CancellationToken cancellationToken = default);
		
	}
}
