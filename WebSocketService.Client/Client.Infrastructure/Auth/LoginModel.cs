using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Client.Infrastructure.Auth
{

	/// <summary>
	/// Модель пользовательских данных
	/// </summary>
	public sealed class LoginModel
	{
		public string UniqueId { get; private set; }
		[Required]
		public string UserName { get; private set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; private set; }

		/// <inheritdoc cref="LoginModel"/>>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="uniqueId"></param>
		[JsonConstructor]
		public LoginModel(string userName, string password, string? uniqueId = null)
		{
			UserName = userName;
			Password = password;
			UniqueId = uniqueId ?? Guid.Empty.ToString();
		}
	}
}
