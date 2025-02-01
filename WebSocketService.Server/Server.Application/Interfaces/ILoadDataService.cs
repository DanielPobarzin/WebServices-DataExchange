namespace Server.Application.Interfaces
{
	/// <summary>
	/// Загрузка данных из базы по типу
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface ILoadDataService<out TEntity> where TEntity : class
	{
		/// <summary>
		/// Получить все данные типа <see cref="TEntity"/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<TEntity> GetAllData();

		/// <summary>
		/// Получение уведомлений от базы данных, транслируемых по каналу
		/// </summary>
		/// <param name="channelName"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task ListenNotification(string channelName, CancellationToken cancellationToken = default);
	}
}
