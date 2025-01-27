namespace Server.Infrastructure.ConfigurationApp
{
	/// <summary>
	/// Настройки служб приложения
	/// </summary>
	public sealed class AppSetting
	{
		/// <summary>
		/// Строка подключения
		/// </summary>
		/// <remarks>
		/// Применительно к источнику данных
		/// </remarks>
		public string ConnectionString { get; private set; } = DefaultAppSetting.ConnectionString;

		/// <summary>
		/// Имя хоста 
		/// </summary>
		public string Host { get; private set; } = DefaultAppSetting.Host;

		/// <summary>
		/// Протокол 
		/// </summary>
		public string Protocol { get; private set; } = DefaultAppSetting.Protocol;

		/// <summary>
		/// Порт
		/// </summary>
		public int[] Port { get; private set; } = DefaultAppSetting.Port;

		/// <summary>
		/// Маршрутизация концентратора
		/// </summary>
		public string RouteHub { get; private set; } = DefaultAppSetting.RouteHub;

		/// <summary>
		/// Временный интервал после закрытия сервера, в течение которого клиент должен закрыть подключение
		/// </summary>
		/// <remarks>
		/// Если клиенту не удастся закрыть подключение, то соединение с сервером автоматически завершается.
		/// </remarks>
		public int CloseTimeout { get; private set; } = DefaultAppSetting.CloseTimeout;

		/// <summary>
		/// Протокол для обращения к шине данных
		/// </summary>
		public string MessageBusProtocol { get; private set; } = DefaultAppSetting.MessageBusProtocol;

		/// <summary>
		/// Имя хоста для обращения к шине данных
		/// </summary>
		public string MessageBusHost { get; private set; } = DefaultAppSetting.MessageBusHost;

		/// <summary>
		/// Порт для обращения к шине данных
		/// </summary>
		public int[] MessageBusPort { get; private set; } = DefaultAppSetting.MessageBusPort;

		/// <summary>
		/// Url
		/// </summary>
		public string[] GetUrls =>
			Port.Select(port => $"{Protocol}://{Host}:{port}").ToArray();

		/// <summary>
		/// Url для обращения к шине данных
		/// </summary>
		public string[] GetMessageBusUrls =>
			Port.Select(port => $"{MessageBusProtocol}://{MessageBusHost}:{port}").ToArray();

		/// <summary>
		/// Изменить настройку
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void SetSetting(string propertyName, object? value)
		{
			var property = typeof(AppSetting).GetProperty(propertyName);
			if (property is null)
				throw new ArgumentNullException();
			property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
		}
	}
}
