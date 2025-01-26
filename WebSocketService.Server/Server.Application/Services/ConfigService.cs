using FunctionalProgramming;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Server.Application.Interfaces.Services;
using Server.Domain.Messages;

namespace Server.Application.Services
{
	/// <inheritdoc cref="IConfigService"/>
	public sealed class ConfigService : IConfigService, IHostedService, IDisposable
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogService _logService;
		private readonly ISelfHost _selfHost;
		private readonly IConfiguration _configuration;
		private readonly Debouncer _debouncer; 

		/// <inheritdoc cref="ConfigService"/>
		public ConfigService(IServiceScopeFactory scopeFactory, ILogService logService, ISelfHost selfHost)
		{
			_scopeFactory = scopeFactory;
			_logService = logService;
			_selfHost = selfHost;
			_debouncer = new Debouncer(TimeSpan.FromSeconds(1), _logService);
			_configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("Config/ServerSettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
		}

		#region implementation IConfigService
		public Task Validation()
		{
			return Task.CompletedTask;
		}

		public IConfiguration GetConfiguration() => _configuration;

		#endregion

		#region implementation IHostedService
		public Task StartAsync(CancellationToken cancellationToken)
		{
			ChangeToken.OnChange(
				() =>
				{
					using var scope = _scopeFactory.CreateScope();
					return _configuration.GetReloadToken();
				},
				changeTokenConsumer: () => _debouncer.Debounce(OnConfigurationReloaded));
			return Task.CompletedTask;
		}
		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

		#endregion

		#region implementation IHostedService
		public void Dispose() => _debouncer.Dispose();

		#endregion

		private void OnConfigurationReloaded() =>
			Task.WhenAll(_selfHost.RebootHostedServiceAsync(_configuration),
				_logService.Write(new LogWarning(WarningLevel.Normal, nameof(ConfigService),
					"Configuration changed. Reboot services ...")));

	}

	/// <summary>
	/// Служба-обёртка, ограничивающая число выполнений переданной в неё функции некоторым промежутком времени
	/// </summary>
	public sealed class Debouncer : IDisposable
	{
		private readonly ILogService _logger;
		private readonly TimeSpan _delay;
		private CancellationTokenSource? _previousCancellationToken;

		/// <inheritdoc cref="Debouncer"/>
		/// <param name="delay">Промежуток времени, в течение которого игнорируются вызовы функции. Если значение равно null,
		/// используется значение по умолчанию 1 секунда.</param>
		/// <param name="logger">Сервис для ведения журнала.</param>
		public Debouncer(TimeSpan? delay, ILogService logger)
		{
			_delay = delay ?? TimeSpan.FromSeconds(1);
			_logger = logger;
		}

		/// <summary>
		/// Выполняет действие с задержкой, игнорируя последующие вызовы до истечения заданного промежутка времени.
		/// </summary>
		/// <param name="action">Действие, которое необходимо выполнить.</param>
		/// <returns>Задача, представляющая асинхронную операцию.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="action"/> является <c>null</c>.</exception>
		public Task Debounce(Action action) => ResultExtensions.TryCatchAsync(
			async () =>
			{
				_ = action ?? throw new ArgumentNullException(nameof(action));
				Dispose();
				_previousCancellationToken = new CancellationTokenSource();

				await Task.Delay(_delay, _previousCancellationToken.Token);
				await Task.Run(action, _previousCancellationToken.Token);

			}).MatchErrorAsync(ex => ex switch
		{
			TaskCanceledException => _logger.Write(new LogWarning(WarningLevel.Low, nameof(Debouncer), ex.Message)),
			_ => _logger.Write(new LogError(ex, nameof(Debouncer), ex.Message))
		});

		/// <summary>
		/// Освобождает все ресурсы, используемые текущим экземпляром <see cref="Debouncer"/>.
		/// </summary>
		public void Dispose()
		{
			if (_previousCancellationToken == null) return;
			_previousCancellationToken.Cancel();
			_previousCancellationToken.Dispose();
		}

	}
}
