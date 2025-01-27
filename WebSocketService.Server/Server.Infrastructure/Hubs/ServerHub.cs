using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Application.Services;
using Server.Domain.Messages;
using Server.Infrastructure.Connection;
using SignalRSwaggerGen.Attributes;

namespace Server.Infrastructure.Hubs
{
	/// <summary>
	/// SignalR hub
	/// </summary>
	[SignalRHub]
	public sealed class ServerHub : Hub
	{
		private readonly ILogService _logger;
		private readonly IConnectionManager _manager;

		/// <inheritdoc cref="ServerHub"/>
		public ServerHub(ILogService logger, IConnectionManager manager)
		{
			_logger = logger;
			_manager = manager;
		}

		/// <summary>
		/// Отправка сообщения всем подписанным клиентам
		/// </summary>
		/// <param name="message">Сообщение для отправки</param>
		[Authorize]
		[SignalRMethod("SendMessageToAll")]
		[HubMethodName("SendMessageToAll")]
		public async Task SendMessage(object message) => 
			await Clients.All.SendAsync("ReceiveMessage", message);
		
		/// <summary>
		/// Отправка сообщения в группу клиентов
		/// </summary>
		/// <param name="userName">Имя пользователя</param>
		/// <param name="message">Сообщение для отправки</param>
		/// <param name="groupName">Имя группы клиентов</param>
		[SignalRMethod("SendMessageToGroup")]
		[HubMethodName("SendMessageToGroup")]
		public async Task SendMessageToGroup(string userName, object message, string? groupName = null) =>
			await Clients.Group((string.IsNullOrEmpty(groupName)) ? "SignalR Users" : groupName)
				.SendAsync("ReceiveMessage", userName, message);

		/// <summary>
		/// Отправка сообщения вызывающему клиенту
		/// </summary>
		/// <param name="userName">Имя пользователя</param>
		/// <param name="message">Сообщение для отправки</param>
		[SignalRMethod("DirectMessage")]
		[HubMethodName("SendMessageToUser")]
		public async Task DirectMessage(string userName, object message) =>
			await Clients.User(userName).SendAsync("ReceiveMessage", userName, message);

		/// <summary>
		/// Вызов исключения на стороне концентратора
		/// </summary>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		/// <exception cref="HubException"></exception>
		public Task ThrowException(string? errorMessage = null)
		{
			_logger.Write(new LogError(new HubException(errorMessage)));
			throw new HubException(errorMessage);
		}

		/// <summary>
		/// Отправка сообщения вызывающему клиенту
		/// </summary>
		/// <param name="userName">Имя пользователя</param>
		/// <param name="message">Сообщение для отправки</param>
		/// <returns>Задача, представляющая асинхронную операцию.</returns>
		[SignalRMethod("SendMessageToCaller")]
		[HubMethodName("SendMessageToCaller")]
		public async Task SendMessageToCaller(string userName, object message) =>
		await Clients.Caller.SendAsync("ReceiveMessage", userName, message);

		/// <summary>
		/// Вызывается при подключении нового клиента к хабу.
		/// Этот метод добавляет клиента в группу "SignalR Users" и уведомляет других клиентов о новом подключении.
		/// </summary>
		/// <returns>Задача, представляющая асинхронную операцию.</returns>
		public override async Task OnConnectedAsync()
		{
			var message = $"A new session has been created and added to 'SignalR Users' group. New connection - {Context.ConnectionId}.";
			await Task.WhenAll(_logger.Write(new LogMessage(message)),
				_manager.AddConnectionBy(connectionId: Context.ConnectionId, groupName: "Private Group", "Anonymous"),
				Clients.Group("Private Group").SendAsync("ReceiveMessage", message));
			await Groups.AddToGroupAsync(Context.ConnectionId, "Private Group");

			await base.OnConnectedAsync();
		}
		/// <summary>
		/// Вызывается при отключении клиента от хаба.
		/// Этот метод удаляет клиента из группы и уведомляет остальных клиентов о разрыве соединения.
		/// </summary>
		/// <param name="exception">Исключение, возникшее перед отключением клиента. Может быть null, если отключение произошло без ошибок.</param>
		/// <returns>Задача, представляющая асинхронную операцию.</returns>
		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			var message = $"Session with {Context.ConnectionId} was closed.";
			await _logger.Write(new LogMessage(message));
			await Task.WhenAll(
				_manager.RemoveConnectionBy(Context.ConnectionId),
				Groups.RemoveFromGroupAsync(Context.ConnectionId, "Private Group"),
				(exception is not null) ? _logger.Write(new LogMessage(exception.Message)) : Task.CompletedTask,
				Clients.Group("Private Group").SendAsync("ReceiveMessage",
				(exception is not null) ? message + $"InnerException: {exception.Message}." : message));
			await base.OnDisconnectedAsync(exception);
		}

		/// <summary>
		/// Вызывается при повторном подключении клиента к хабу.
		/// Этот метод уведомляет других клиентов о том, что клиент снова подключился.
		/// </summary>
		/// <returns>Задача, представляющая асинхронную операцию.</returns>
		public async Task OnReconnectedAsync()
		{
			await Clients.Others.SendAsync("ReceiveMessage", $"{Context.ConnectionId} reconnected to group.");
		}
	}
}