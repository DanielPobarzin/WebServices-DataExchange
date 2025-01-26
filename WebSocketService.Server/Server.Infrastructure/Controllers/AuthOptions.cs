using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Server.Infrastructure.Controllers;

/// <summary>
/// Настройки генерации токена
/// </summary>
internal static class AuthOptions
{
	/// <summary>
	/// Издатель токена авторизации
	/// </summary>
	internal const string Issuer = "AuthServer";
	/// <summary>
	/// Потребитель токена авторизации
	/// </summary>
	internal const string Audience = "AuthClient";
	/// <summary>
	/// Ключ шифрации
	/// </summary>
	private const string Key = "DfGhNbvRTghqW_f432FDew!&Gfvbnmk8Ut54Ew2";
	/// <summary>
	/// Получить ключ защиты
	/// </summary>
	/// <returns><see cref="SymmetricSecurityKey"/></returns>
	public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
}