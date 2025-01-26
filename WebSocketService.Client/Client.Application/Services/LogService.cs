using Client.Application.Interfaces.Services;
using Client.Domain.Messages;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Client.Application.Services;

/// <inheritdoc cref="ILogService"/>
public sealed class LogService: ILogService
{
	/// <inheritdoc cref="LogService"/>>
	public LogService()
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
			.Enrich.FromLogContext()
			.Enrich.WithExceptionData()
			.Enrich.WithMemoryUsage()
			.Enrich.WithProcessName()
			.Enrich.WithThreadName()
			.Enrich.WithDemystifiedStackTraces()
			.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Information)
			.WriteTo.File(AppContext.BaseDirectory + @"\Log\[VERBOSE]_Message_Exchange_Log_.log",
				fileSizeLimitBytes: 100000000,
				rollingInterval: RollingInterval.Day,
				rollOnFileSizeLimit: true,
				retainedFileCountLimit: 365,
				shared: true,
				restrictedToMinimumLevel: LogEventLevel.Verbose)
			.WriteTo.File(AppContext.BaseDirectory + @"\Log\[ERROR]_Message_Exchange_Log_.log",
				fileSizeLimitBytes: 100000000,
				rollingInterval: RollingInterval.Day,
				rollOnFileSizeLimit: true,
				retainedFileCountLimit: 365,
				shared: true,
				restrictedToMinimumLevel: LogEventLevel.Error)
			.WriteTo.File(AppContext.BaseDirectory + @"\Log\[INFO]_Message_Exchange_Log_.log",
				fileSizeLimitBytes: 100000000,
				rollingInterval: RollingInterval.Day,
				rollOnFileSizeLimit: true,
				retainedFileCountLimit: 365,
				shared: true,
				restrictedToMinimumLevel: LogEventLevel.Information)
			.CreateLogger();
	}
	/// <summary>
	/// Запись в журнал сообщений
	/// </summary>
	/// <param name="msg"></param>
	/// <returns></returns>
	public Task Write(LogMessage msg)
	{
		switch (msg)
		{
			case LogWarning warning:
				Log.Warning(warning.Text, warning.Arguments);
				break;
			case LogError error:
				Log.Error(error.Text, error.Arguments);
				break;
			case not null:
				Log.Information(msg.Text, msg.Arguments);
				break;
		}
		return Task.CompletedTask;
	}

	#region implementation ILogger
	public void Write(LogEvent logEvent) => throw new NotSupportedException();

	#endregion
}