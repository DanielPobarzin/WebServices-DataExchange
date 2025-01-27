using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Server.Application.Services;
using Server.Domain.Messages;
using System.Reflection;

namespace Server.Infrastructure.ConfigurationApp;

/// <summary>
/// Служба предоставления конфигурации приложения.
/// </summary>
public interface IConfigService
{
	/// <summary>
	/// Запросить у службы текущую конфигурацию приложения.
	/// </summary>
	/// <returns></returns>
	AppSetting GetConfiguration();
	/// <summary>
	/// Запуск фоновой службы отслеживания изменения конфигурации.
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task StartAsync(CancellationToken cancellationToken = default);

}

/// <inheritdoc cref="IConfigService"/>
public sealed class ConfigService : IConfigService, IHostedService, IDisposable
{
	private readonly ILogService _logger;
	private readonly ISelfHost _selfHost;
	private readonly IConfiguration _configuration;
	private readonly AppSettingValidator _validator;
	private readonly Debouncer _debouncer;

	/// <inheritdoc cref="ConfigService"/>
	public ConfigService(ILogService logService, ISelfHost selfHost)
	{
		_validator = new();
		_logger = logService;
		_selfHost = selfHost;
		_debouncer = new Debouncer(TimeSpan.FromSeconds(1), _logger);
		_configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("Config/ServerSettings.json", optional: false, reloadOnChange: true)
			.AddEnvironmentVariables()
			.Build();
	}

	#region implementation IConfigService

	public AppSetting GetConfiguration()
	{
		var setting = new AppSetting();
		foreach (var property in typeof(AppSetting).GetProperties(BindingFlags.Public | BindingFlags.Instance)
			         .Where(p => p.CanWrite))
			setting.SetSetting(property.Name, _configuration.GetSection(property.Name).Get(property.PropertyType));

		return Validation(setting);
	}

	#endregion

	#region implementation IHostedService
	public Task StartAsync(CancellationToken cancellationToken)
	{
		ChangeToken.OnChange(() => _configuration.GetReloadToken(),
			changeTokenConsumer: () => _debouncer.Debounce(OnConfigurationReloaded));
		return Task.CompletedTask;
	}
	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	#endregion

	#region implementation IDisposable
	public void Dispose() => _debouncer.Dispose();

	#endregion

	private AppSetting Validation(AppSetting setting)
	{
		var result = _validator.Validate(setting);
		if (result.IsValid) return setting;

		result.Errors.ForEach(error =>
		{
			var defaultProperty = typeof(DefaultAppSetting).GetProperty(error.PropertyName);
			if (defaultProperty is null) throw new InvalidOperationException();

			var defaultValue = defaultProperty.GetValue(typeof(DefaultAppSetting));
			setting.SetSetting(error.PropertyName, defaultValue);

			_logger.Write(new LogWarning(sender: nameof(ConfigService),
				text: error.ErrorMessage +
				      $" Default value {defaultValue} is used."));
		});

		return setting;
	}

	private void OnConfigurationReloaded() =>
		Task.WhenAll(_selfHost.RebootHostedServiceAsync(GetConfiguration()),
			_logger.Write(new LogWarning(WarningLevel.Normal, nameof(ConfigService),
				"Configuration changed. Reboot services ...")));

}
