using System.Collections.Concurrent;

namespace Server.Infrastructure.Connection
{
	/// <summary>
	/// Менеджер подключений к концентратору SignalR.
	/// </summary>
	public interface IConnectionManager
	{
		/// <summary>
		/// Добавить новое подключение по уникальному идентификатору подключения
		/// </summary>
		/// <param name="connectionId">Идентификатор подключений</param>
		/// <param name="groupName">Имя группы клиентов</param>
		/// <param name="user">Имя пользователя</param>
		Task AddConnectionBy(string connectionId, string? groupName, string? user);

		/// <summary>
		/// Удалить подключение по уникальному идентификатору подключения
		/// </summary>
		/// <param name="connectionId">Идентификатор подключений</param>
		/// <returns></returns>
		Task RemoveConnectionBy(string connectionId);

		/// <summary>
		/// Запрос идентификатора подключения
		/// </summary>
		/// <param name="user">Имя пользователя</param>
		/// <returns>Идентификатор подключения</returns>
		string GetConnectionIdBy(string user);

		/// <summary>
		/// Запрос всех идентификаторов подключений
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetAllConnectionIds();
	}

	/// <inheritdoc cref="IConnectionManager"/>
	internal sealed class ConnectionManager : IConnectionManager
	{
		private readonly ConcurrentDictionary<string, string> _allClients = new();
		private readonly ConcurrentDictionary<string, KeyValuePair<string, string>> _allGroups = new();

		public Task AddConnectionBy(string connectionId, string? groupName = null, string? user = null)
		{
			_allClients[connectionId] = user ?? string.Empty;
			_allGroups[groupName ?? "Public group"] = new(connectionId, user ?? string.Empty);
			return Task.CompletedTask;
		}

		public Task RemoveConnectionBy(string connectionId)
		{
			_allClients.TryRemove(connectionId, out _);
			_allGroups
				.Where(kvp => kvp.Value.Key.Equals(connectionId))
				.Select(kvp => kvp.Key)
				.ToList().ForEach(key => _allGroups.TryRemove(key, out _));
			return Task.CompletedTask;
		}

		public string GetConnectionIdBy(string user) =>
			_allClients.SingleOrDefault(p => p.Value == user).Key 
			?? throw new ArgumentNullException();
		
		public IEnumerable<string> GetAllConnectionIds() => _allClients.Keys.ToList();
	}
}
