using System.ComponentModel.DataAnnotations;

namespace Server.Infrastructure.Connection
{
	/// <summary>
	/// Модель данных пользователя
	/// </summary>
	public sealed class LoginModel
	{
		[Required]
		public string UserName { get; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; }

		/// <inheritdoc cref="LoginModel"/>>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		public LoginModel(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}
	}
}
