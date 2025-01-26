namespace Client.Domain.Library
{
	/// <summary>
	/// Сообщение
	/// </summary>
	public abstract class Message: ValueObject<Message>
	{
		/// <summary>
		/// Содержание сообщения
		/// </summary>
		public string Content { get; }

		/// <summary>
		/// Время отправки сообщения
		/// </summary>
		public DateTime TimeStamp { get; }

		/// <inheritdoc cref="Message"/>
		protected Message(string content)
		{
			Content = content;
			TimeStamp = DateTime.UtcNow;
		}
	}
}

