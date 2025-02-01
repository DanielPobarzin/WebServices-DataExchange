using Microsoft.AspNetCore.SignalR;
using Moq;
using Server.Application.Services;
using Server.Domain.Messages;
using Server.Infrastructure.Connection;
using Server.Infrastructure.Hubs;

namespace Server.Tests;

public class HubTests
{
	[Fact]
	public async Task SignalR_OnSendMessage_ShouldSendMessage()
	{
		const string message = "message";
		var mockClientProxy = CreateMockClientProxy();
		var mockClients = CreateMockClients();
		mockClients.Setup(clients => clients.All)
			.Returns(mockClientProxy.Object);
		var simpleHub = CreateServerHub(mockClients);

		await simpleHub.SendMessage(message);

		mockClients.Verify(clients => clients.All, Times.Once);
		mockClientProxy.Verify(
			clientProxy => clientProxy.SendCoreAsync(
				"ReceiveMessage", It.Is<object[]>(msg => (string)msg[0] == message),
				CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task SignalR_OnSendMessage_ShouldSendNotification()
	{
		var notification = CreateNotificationMessage(true, content: "content");
		var mockClientProxy = CreateMockClientProxy();
		var mockClients = CreateMockClients();
		mockClients.Setup(clients => clients.All)
			.Returns(mockClientProxy.Object);
		var simpleHub = CreateServerHub(mockClients);

		await simpleHub.SendMessage(notification);

		mockClientProxy.Verify(
			clientProxy => clientProxy.SendCoreAsync(
				"ReceiveMessage", It.Is<object[]>(msg => (Notification)msg[0] == notification),
				CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task SignalR_OnSendMessageToGroup_ShouldSendNotification()
	{
		const string groupName = "groupName";
		var notification = CreateNotificationMessage(true, content: "content");
		var mockClientProxy = CreateMockClientProxy();
		var mockClients = CreateMockClients();
		mockClients.Setup(clients => clients.Group(groupName))
			.Returns(mockClientProxy.Object);
		var hub = CreateServerHub(mockClients);
		await hub.Groups.AddToGroupAsync(Guid.Empty.ToString(), groupName);

		await hub.SendMessageToGroup("userName",notification, groupName);

		mockClientProxy.Verify(
			clientProxy => clientProxy.SendCoreAsync(
				"ReceiveMessage", It.Is<object[]>(msg =>
					msg.Length == 2 &&
					(string)msg[0] == "userName" &&
					(Notification)msg[1] == notification),
				CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task SignalR_OnSendMessageToCaller_ShouldSendNotification()
	{
		var notification = CreateNotificationMessage(true, content: "content");
		var mockClientProxy = CreateMockSingleClientProxy();
		var mockClients = CreateMockClients();
		mockClients.Setup(clients => clients.Caller)
			.Returns(mockClientProxy.Object);
		var hub = CreateServerHub(mockClients);

		await hub.SendMessageToCaller("userName", notification);

		mockClients.Verify(clients => clients.Caller, Times.Once);
		mockClientProxy.Verify(
			clientProxy => clientProxy.SendCoreAsync(
				"ReceiveMessage", It.Is<object[]>(msg =>
					(string)msg[0] == "userName" &&
					(Notification)msg[1] == notification),
				CancellationToken.None), Times.Once);
	}
	public static ServerHub CreateServerHub(Mock<IHubCallerClients>? clients = null) => new(Mock.Of<ILogService>(), Mock.Of<IConnectionManager>())
	{
		Clients = (clients is not null) ? clients.Object : Mock.Of<IHubCallerClients>(),
		Groups = new Mock<IGroupManager>().Object
	};

	public static Mock<IHubCallerClients> CreateMockClients() => new();

	public static Mock<IClientProxy> CreateMockClientProxy() => new();

	public static Mock<ISingleClientProxy> CreateMockSingleClientProxy() => new();

	public static Notification CreateNotificationMessage(bool quality, string? content = null, double? value = null) =>
		new(content, value, quality);
}