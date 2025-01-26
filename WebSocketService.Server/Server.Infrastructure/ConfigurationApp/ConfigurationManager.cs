using Server.Application.Interfaces.Services;

namespace Server.Infrastructure.ConfigurationApp
{
	/// <summary>
	///  Менеджер конфигураций
	/// </summary>
	internal sealed class ConfigurationManager
	{
		private readonly ILogService _logService;

		/// <inheritdoc cref="ConfigurationManager"/>>
		/// <param name="logService"></param>
		public ConfigurationManager(ILogService logService)
		{
			_logService = logService;
		}

	}
}
