using Client.Application.Interfaces.Services;
using Client.Domain.Messages;
using FunctionalProgramming;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client.Infrastructure
{
	/// <summary>
	/// Обслуживание подключения клиента к концентратору
	/// </summary>
	public interface IClientHubConnection
	{
		/// <summary>
		/// Выполнить подлкючение и начать сессию
		/// </summary>
		/// <param name="url"></param>
		/// <param name="tokenProvider">поставщик JWT токена аутентификации</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task StartSessionAsync(string url, Func<Task<string?>> tokenProvider,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Принудительно завершить сессию 
		/// </summary>
		/// <returns></returns>
		Task StopSessionAsync();

		/// <summary>
		/// Состояние подключения
		/// </summary>
		/// <returns></returns>
		HubConnectionState? StateSession();

		/// <summary>
		/// Отправить хабу на сообщение
		/// </summary>
		/// <param name="hubMethodName">Имя метода хаба для обработки сообщений</param>
		/// <param name="message"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task SendToHubAsync(string hubMethodName, object?[] message, CancellationToken cancellationToken = default);

	}

	/// <inheritdoc cref="IClientHubConnection"/>
	internal sealed class ClientHubConnection(ILogService logger) : IClientHubConnection
	{
		private HubConnection? _hubConnection;

		private static Task<HubConnection> CreateConnectionBy(string url, Func<Task<string?>> tokenProvider) =>
			Task.FromResult(new HubConnectionBuilder()
				.AddJsonProtocol(options => options.PayloadSerializerOptions.WriteIndented = true)
				.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Information))
				.WithUrl(url, options =>
				{
					options.AccessTokenProvider = tokenProvider;
					options.SkipNegotiation = false;
					options.ClientCertificates =
						new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
					options.CloseTimeout = TimeSpan.FromSeconds(15);
					options.DefaultTransferFormat = TransferFormat.Text;
					options.UseDefaultCredentials = true;
				})
				.WithKeepAliveInterval(TimeSpan.FromSeconds(60))
				.WithServerTimeout(TimeSpan.FromMinutes(30))
				.WithAutomaticReconnect()
				.Build());

		public Task StartSessionAsync(string url, Func<Task<string?>> tokenProvider,
			CancellationToken cancellationToken = default) => ResultExtensions.TryCatchAsync(
			async () =>
			{
				_hubConnection = await CreateConnectionBy(url, tokenProvider);

				await _hubConnection.StartAsync(cancellationToken);
				{
					_hubConnection.On<object>("ReceiveMessage", OnReceiveFromHabMessage);
				}
			
				_hubConnection.Closed += OnConnectionClosed;
				_hubConnection.Reconnected += OnConnectionReconnected;
				_hubConnection.Reconnecting += OnConnectionReconnecting;

			}, cancellationToken).MatchErrorAsync(async ex =>
			await logger.Write(new LogError(ex, $"ClientHubConnection.Method{nameof(StartSessionAsync)}", ex.Message)), token: cancellationToken);
		
		public Task StopSessionAsync()
		{
			throw new NotImplementedException();
		}

		public HubConnectionState? StateSession() => _hubConnection?.State;

		public Task SendToHubAsync(string hubMethodName, object?[] message,
			CancellationToken cancellationToken = default) =>
			ResultExtensions.TryCatchAsync(
				async () =>
				{
					if (_hubConnection is null)
						throw new InvalidOperationException(nameof(HubConnection));

					await _hubConnection.InvokeAsync(hubMethodName, message, cancellationToken: cancellationToken);
				}, cancellationToken).MatchErrorAsync(async ex =>
				await logger.Write(new LogError(ex, $"ClientHubConnection.Method{nameof(SendToHubAsync)}",
					ex.Message)), token: cancellationToken);

		private static void OnReceiveFromHabMessage(object message) {Console.WriteLine(message.ToString());}
		private Task OnConnectionClosed(Exception? ex)
		{
			Console.WriteLine(ex?.Message);
			Console.WriteLine("closed");
			return Task.CompletedTask;
		}

		private static Task OnConnectionReconnecting(Exception? ex)
		{
			Console.WriteLine(ex?.Message);
			Console.WriteLine("Reconnecting");
			return Task.CompletedTask;
		}
		private Task OnConnectionReconnected(string? arg)
		{
			Console.WriteLine("Reconnected");
			return Task.CompletedTask;
		}

	}
}