using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Server.Application.ConfigurationApp;
using Server.Domain.Messages;
using System.Reflection;

namespace Server.Application.Services;

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
	/// <summary>
    /// Выполнить подписку
    /// </summary>
    /// <param name="observer"></param>
    /// <returns></returns>
    IDisposable Subscribe(IObserver<AppSetting> observer);
}

/// <inheritdoc cref="IConfigService"/>
public sealed class ConfigService : IConfigService, IHostedService, IObservable<AppSetting>, IDisposable
{
    private readonly ILogService _logger;
    private readonly IConfiguration _configuration;
    private readonly AppSettingValidator _validator;
    private readonly Debouncer _debouncer;
    private readonly List<IObserver<AppSetting>> _observers;

	/// <inheritdoc cref="ConfigService"/>
	public ConfigService(ILogService logService)
    {
        _validator = new();
        _logger = logService;
        _debouncer = new Debouncer(TimeSpan.FromSeconds(1), _logger);
        _observers = new();
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

	#region implementation IObservable

	/// <inheritdoc cref="IObservable{AppSetting}"/>
	public IDisposable Subscribe(IObserver<AppSetting> observer)
	{
		if (!_observers.Contains(observer))
			_observers.Add(observer);

		return new Unsubscriber(_observers, observer);
	}

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

	private void NotifyObservers(AppSetting newConfig) =>
		_observers.ForEach(observer => observer.OnNext(newConfig));

	private void OnConfigurationReloaded()
	{
		NotifyObservers(GetConfiguration());
		_logger.Write(new LogWarning(WarningLevel.Normal, nameof(ConfigService),
			"Configuration changed. Notifying observers..."));
	}

	private sealed class Unsubscriber(List<IObserver<AppSetting>> observers, IObserver<AppSetting> observer)
	    : IDisposable
    {
	    public void Dispose()
	    {
		    if (observers.Contains(observer)) 
			    observers.Remove(observer);
	    }
    }
}
