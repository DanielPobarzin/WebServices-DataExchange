using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Infrastructure.Connection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Server.Infrastructure.Controllers
{
	/// <summary>
	/// Контроллер авторизации
	/// </summary>
	[ApiController]
	[Route("api/v1/[controller]")]
	public sealed class AuthController : ControllerBase
	{
		/// <summary>
		/// Выполнить авторизацию пользователя
		/// </summary>
		[HttpPost, Route("login")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[SwaggerResponse(200, "Авторизация успешна", typeof(string))]
		[SwaggerResponse(400, "Некорректные учетные данные", typeof(string))]
		[SwaggerResponse(401, "Авторизация не выполнена", typeof(string))]
		public IActionResult Login([FromBody] LoginModel loginModel)
		{
			if (string.IsNullOrEmpty(loginModel.UserName) || 
			    string.IsNullOrEmpty(loginModel.Password))
				return BadRequest();
			
			//Person? person = people.FirstOrDefault(p => p.Email == loginModel.Email && p.Password == loginModel.Password);
			//if (person is null) return Results.Unauthorized();

			var claims = new List<Claim> { new(ClaimTypes.Name, loginModel.UserName) };
			var jwt = new JwtSecurityToken(
					issuer: AuthOptions.Issuer,
					audience: AuthOptions.Audience,
					claims: claims,
					expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
					signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

			return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
			
		}
	}
}