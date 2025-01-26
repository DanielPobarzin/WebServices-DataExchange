using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Server.Infrastructure.Controllers;
using Server.Infrastructure.Hubs;

namespace Server.Tests;

public class ControllerTests
{
	[Fact]
	public void PingServer()
	{
		var controller = CreateServerController();

		var result = controller.Ping();

		Assert.Multiple(() =>
		{
			Assert.NotNull(result);
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			Assert.Equal("Echo", okResult.Value);
		});

	}


	private static ServerController CreateServerController() => new(Mock.Of<IHubContext<ServerHub>>());
}