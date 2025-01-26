using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Domain.Messages;
using Server.Infrastructure.Hubs;
using Swashbuckle.AspNetCore.Annotations;

namespace Server.Infrastructure.Controllers
{
	/// <summary>
	/// Контроллер для переправления запроса в концентратор 
	/// </summary>
	/// <param name="hubContext"></param>
	[ApiController]
	[Produces("application/json")]
	[Route("api/v1/serverHub/[controller]")]
	public class ServerController(IHubContext<ServerHub> hubContext) : ControllerBase
	{
		/// <summary>
		/// Запрос информации с сервера
		/// </summary>
		/// <returns>Возврат тестового значения</returns>
		/// <response code="200">Успешно</response>
		/// <response code="400">Некорректный запрос</response>
		/// <response code="401">Авторизация не выполнена</response>
		/// <response code="500">Ошибка на стороне сервера</response>
		[Authorize]
		[HttpGet("ping")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[SwaggerResponse(200, "Successfully Request", typeof(string))]
		[SwaggerResponse(400, "Bad Request", typeof(string))]
		[SwaggerResponse(401, "Authorization failed", typeof(string))]
		[SwaggerResponse(500, "Internal Server Error", typeof(string))]
		public ActionResult<string> Ping()
		{
			return Ok("Echo");
		}


		/// <summary>
		/// Отправляет сообщение всем подключенным клиентам.
		/// </summary>
		/// <param name="message">Сообщение для отправки.</param>
		/// <returns></returns>
		/// <response code="200">Сообщение успешно отправлено</response>
		/// <response code="400">Некорректный запрос</response>
		/// <response code="401">Авторизация не выполнена</response>
		/// <response code="500">Ошибка на стороне сервера</response>
		[Authorize]
		[HttpPost("send-message-to-all")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[SwaggerResponse(200, "Message was sent successfully", typeof(string))]
		[SwaggerResponse(400, "Bad Request", typeof(string))]
		[SwaggerResponse(401, "Authorization failed", typeof(string))]
		[SwaggerResponse(500, "Internal Server Error", typeof(string))]
		public async Task<IActionResult> SendMessageToAll([FromBody] string message)
		{
			await hubContext.Clients.All.SendAsync("Receive", $"{message} - {DateTime.Now.ToLongTimeString()}");
			return Ok();
		}

		/// <summary>
		/// Отправляет уведомление конкретному клиенту по идентификатору подключения.
		/// </summary>
		/// <param name="connectionId">Идентификатор подключения.</param>
		/// <param name="notification">Уведомление.</param>
		/// <returns>UId подключения.</returns>
		/// <response code="200">Сообщение успешно отправлено</response>
		/// <response code="400">Некорректный запрос</response>
		/// <response code="401">Авторизация не выполнена</response>
		/// <response code="500">Ошибка на стороне сервера</response>
		[Authorize]
		[HttpPost("send-notification-by/{connectionId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[SwaggerResponse(200, "Message was sent successfully", typeof(string))]
		[SwaggerResponse(400, "Bad Request", typeof(string))]
		[SwaggerResponse(401, "Authorization failed", typeof(string))]
		[SwaggerResponse(500, "Internal Server Error", typeof(string))]
		public async Task<ActionResult<Guid>> SendMessageBy([FromRoute] string connectionId, [FromBody] Notification notification)
		{
			if (string.IsNullOrEmpty(connectionId))
				return BadRequest("Идентификатор подключения не определен.");
			await hubContext.Clients.Client(connectionId).SendAsync("Notify", notification);
			return Ok(connectionId);
		}
	}
}