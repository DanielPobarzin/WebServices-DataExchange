using FunctionalProgramming;
using Server.Application.Services;
using Server.Domain.Messages;

namespace Server.Application.ConfigurationApp
{
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
