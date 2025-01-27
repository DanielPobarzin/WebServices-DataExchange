namespace Server.Infrastructure.ConfigurationApp;

/// <summary>
/// Настройки по умолчанию служб приложения
/// </summary>
internal static class DefaultAppSetting
{
	/// <summary>
	/// Строка подключения
	/// </summary>
	/// <remarks>
	/// Применительно к источнику данных
	/// </remarks>
	public static string ConnectionString => "host=localhost;port=5432;Database=AlarmsExchange;Username=postgres;Password=19346jaidj";
	/// <summary>
	/// Имя хоста 
	/// </summary>
	public static string Host => "localhost";
	/// <summary>
	/// Протокол 
	/// </summary>
	public static string Protocol => "https";
	/// <summary>
	/// Порт
	/// </summary>
	public static int[] Port => [8080];
	/// <summary>
	/// Маршрутизация концентратора
	/// </summary>
	public static string RouteHub => "/hub";
	/// <summary>
	/// Временный интервал после закрытия сервера, в течение которого клиент должен закрыть подключение
	/// </summary>
	/// <remarks>
	/// Если клиенту не удастся закрыть подключение, то соединение с сервером автоматически завершается.
	/// </remarks>
	public static int CloseTimeout => 15;
	/// <summary>
	/// Протокол для обращения к шине данных
	/// </summary>
	public static string MessageBusProtocol => "https";
	/// <summary>
	/// Имя хоста для обращения к шине данных
	/// </summary>
	public static string MessageBusHost => "localhost";
	/// <summary>
	/// Порт для обращения к шине данных
	/// </summary>
	public static int[] MessageBusPort => [3050, 3052];
}