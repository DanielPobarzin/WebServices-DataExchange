namespace Server.DataApplicationLayer.Messages.Results
{
	/// <summary>
	/// Результат операции сохранения изменений
	/// </summary>
	public sealed class SaveChangesResult
	{
		/// <summary>
		/// Сообщения об ошибках
		/// </summary>
		private List<string> Messages { get; }
		/// <summary>
		/// Исключение
		/// </summary>
		public System.Exception? Exception { get; set; }

		/// <summary>
		/// Занято ли исключение во время выполнения последней операции
		/// </summary>
		public bool IsOk => Exception == null;

		/// <summary>
		/// Добавить сообщение в результат
		/// </summary>
		/// <param name="message"></param>
		public void AddMessage(string message) => Messages.Add(message);

		/// <inheritdoc cref="SaveChangesResult"/>
		public SaveChangesResult() => Messages = [];

		/// <inheritdoc cref="SaveChangesResult"/>
		public SaveChangesResult(string message) : this() => AddMessage(message);

		
	}
}
