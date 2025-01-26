using Client.Application.Interfaces.Services;
using Client.Application.Services;
using Client.Infrastructure.Auth;

namespace Client.Infrastructure;

public class Program
{
	public static async Task Main(string[] args)
	{
		var loginModel = new LoginModel("your_username", "your_password");
		ILogService logService = new LogService();
		var client = new AuthClient(logService);
		IClientHubConnection hubConnection = new ClientHubConnection(logService);

		await hubConnection.StartSessionAsync("https://localhost:8080/hub", () => client.AuthenticateUserAsync("https://localhost:8080/api/v1/Auth/login", loginModel));

		await hubConnection.SendToHubAsync("SendMessageToAll", ["fuck my ass"]);


		Console.ReadKey();
	}
}