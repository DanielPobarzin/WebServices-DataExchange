namespace Client.Domain.Messages
{
	/// <summary>Сообщение ведения журнала</summary>
	public record LogMessage 
	{
		/// <summary>Писатель</summary>
		public string? Sender { get; }

		/// <summary>Текст лога</summary>
		public string Text { get; }

		/// <summary>Агрументы сообщения</summary>
		public object[] Arguments { get; }

		/// <inheritdoc cref="T:AgentModel.LogMessage" />
		/// <param name="sender"></param>
		/// <param name="text"></param>
		/// <param name="arguments"></param>
		public LogMessage(string text, string? sender = null, params object[] arguments)
		{ 
			Sender = sender ?? string.Empty;
			Text =	sender switch
			{
				null => $"Message: {text}",
				not null => $"Message: {text}  Sender: {sender}"
			};
			Arguments = arguments;
		}
	}

	/// <summary>Ошибка для записи в журнал</summary>
	public record LogError : LogMessage
	{
		/// <summary>Исключение</summary>
		public Exception Exception { get; }

		/// <inheritdoc cref="LogError" />
		/// <param name="exception"></param>
		/// <param name="sender"></param>
		/// <param name="text"></param>
		/// <param name="arguments"></param>
		public LogError(Exception exception, string? sender = null, string? text = null, params object[] arguments)
			: base(text ?? string.Empty, sender, arguments)
		{
			this.Exception = exception;
		}
	}

	/// <summary>Предупреждение для записи в журнал</summary>
	public record LogWarning : LogMessage
	{
		/// <summary>Уровень предупреждения</summary>
		public WarningLevel WarningLevel { get; }

		/// <inheritdoc cref="LogWarning" />
		/// <param name="level"></param>
		/// <param name="sender"></param>
		/// <param name="text"></param>
		/// <param name="arguments"></param>
		public LogWarning(WarningLevel? level = null, string? sender = null, string? text = null, params object[] arguments)
			: base(text ?? string.Empty, sender, arguments)
		{
			WarningLevel = level ?? WarningLevel.Normal;
		}
	}
	public enum WarningLevel
	{
		Low = 0, 
		Normal = 1, 
		Tall = 2, 
		Critical = 3
	}
}
