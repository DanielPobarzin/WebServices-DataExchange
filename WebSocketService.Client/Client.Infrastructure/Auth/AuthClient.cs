using Client.Application.Interfaces.Services;
using Client.Domain.Messages;
using FunctionalProgramming;
using Newtonsoft.Json;
using System.Net;
using System.Security;
using System.Text;

namespace Client.Infrastructure.Auth
{
	/// <summary>
	/// Служба аутентификации с использованием JWT
	/// </summary>
	/// <param name="logService"></param>
	internal sealed class AuthClient(ILogService logService)
	{
		/// <summary>
		/// Выполнить аутентификацию пользователя <see cref="LoginModel"/> по адресу <see cref="url"/>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="loginModel"></param>
		/// <returns>Токен JWT</returns>
		public Task<string?> AuthenticateUserAsync(string url, LoginModel loginModel) => ResultExtensions.TryCatchAsync(
			async () =>
			{
				using var httpClient = new HttpClient();
				using var response = await httpClient.PostAsync(url, new StringContent(
					JsonConvert.SerializeObject(loginModel), Encoding.UTF8,
					"application/json"));

				if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();
				throw response.StatusCode switch
				{
					HttpStatusCode.BadRequest => new ArgumentException("Invalid request."),
					HttpStatusCode.Unauthorized => new UnauthorizedAccessException("Authentication failed."),
					HttpStatusCode.Forbidden => new SecurityException("Access to the resource is forbidden."),
					HttpStatusCode.NotFound => new KeyNotFoundException("Resource not found."),
					HttpStatusCode.InternalServerError => new InvalidOperationException("An error occurred on the server."),
					_ => new Exception("An unknown error occurred.")
				};
			}).MatchAsync(
			actionError: ex =>
			{
				logService.Write(new LogError(ex, nameof(AuthClient), ex.Message));
				return Task.FromResult(string.Empty);
			},
			actionOk: Task.FromResult)!;
	}
}