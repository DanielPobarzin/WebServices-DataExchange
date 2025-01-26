using Client.Domain.Messages;
using Serilog;

namespace Client.Application.Interfaces.Services
{
	/// <summary>
	/// Служба логгирования
	/// </summary>
	public interface ILogService : ILogger
	{
		/// <summary>
		/// Выполнить запись сообщения
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		Task Write(LogMessage msg);
	}
}
