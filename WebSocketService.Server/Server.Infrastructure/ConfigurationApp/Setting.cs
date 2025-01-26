using System.Text.Json.Serialization;

namespace Server.Infrastructure.ConfigurationApp
{
	internal sealed class Setting
	{
		[JsonPropertyName("urls")]
		public required string Urls { get; init; }
	}
}
