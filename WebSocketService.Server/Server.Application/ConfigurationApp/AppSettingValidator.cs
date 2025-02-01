using FluentValidation;

namespace Server.Application.ConfigurationApp;

/// <summary>
/// Валидатор для проверки корректности настроек служб приложения
/// </summary>
internal sealed class AppSettingValidator : AbstractValidator<AppSetting>
{
	/// <inheritdoc cref="AppSettingValidator"/>
	internal AppSettingValidator()
	{
		RuleFor(x => x.ConnectionString)
			.NotEmpty().WithMessage("Connection string cannot be empty.")
			.Matches(@"^host=.*;port=.*;Database=.*;Username=.*;Password=.*$")
			.WithMessage(
				"Connection string format is invalid. Expected format: 'host=...;port=...;Database=...;Username=...;Password=...'.");

		RuleFor(x => x.AuthDbConnectionString)
			.NotEmpty().WithMessage("Connection string to AuthDb cannot be empty.")
			.Matches(@"^host=.*;port=.*;Database=.*;Username=.*;Password=.*$")
			.WithMessage(
				"AuthDb connection string format is invalid. Expected format: 'host=...;port=...;Database=...;Username=...;Password=...'.");

		RuleFor(x => x.Host)
			.NotEmpty().WithMessage("Host cannot be empty.")
			.Matches(@"^(|[a-zA-Z0-9.-]+)$")
			.WithMessage("Host must be a valid hostname.");

		RuleFor(x => x.Protocol)
			.NotEmpty().WithMessage("Protocol cannot be empty.")
			.Must(protocol => protocol is "http" or "https")
			.WithMessage("Protocol must be either 'http' or 'https'.");

		RuleFor(x => x.Port)
			.NotNull().WithMessage("Port cannot be null.")
			.Must(ports => ports.Length > 0).WithMessage("At least one port must be specified.")
			.Must(ports => ports.All(port => port is > 0 and <= 65535))
			.WithMessage("All ports must be valid (1-65535).")
			.Must(ports => ports.Distinct().Count() == ports.Length)
			.WithMessage("Ports must be unique.");

		RuleFor(x => x.RouteHub)
			.NotEmpty().WithMessage("RouteHub cannot be empty.")
			.Matches(@"^/[a-zA-Z0-9/]*$")
			.WithMessage("RouteHub must start with '/' and contain only alphanumeric characters and slashes.")
			.Must(route => !route.EndsWith("/"))
			.WithMessage("RouteHub must not end with a slash.");

		RuleFor(x => x.CloseTimeout)
			.GreaterThan(0).WithMessage("CloseTimeout must be greater than zero.")
			.LessThanOrEqualTo(60).WithMessage("CloseTimeout must be less than or equal to 60 seconds.");

		RuleFor(x => x.MessageBusProtocol)
			.NotEmpty().WithMessage("MessageBusProtocol cannot be empty.")
			.Must(protocol => protocol is "http" or "https")
			.WithMessage("MessageBusProtocol must be either 'http' or 'https'.");

		RuleFor(x => x.MessageBusHost)
			.NotEmpty().WithMessage("MessageBusHost cannot be empty.")
			.Matches(@"^(|[a-zA-Z0-9.-]+)$")
			.WithMessage("MessageBusHost must be a valid hostname.");

		RuleFor(x => x.MessageBusPort)
			.NotNull().WithMessage("MessageBusPort cannot be null.")
			.Must(ports => ports.Length > 0).WithMessage("At least one MessageBus port must be specified.")
			.Must(ports => ports.All(port => port is > 0 and <= 65535))
			.WithMessage("All MessageBus ports must be valid (1-65535).")
			.Must(ports => ports.Distinct().Count() == ports.Length)
			.WithMessage("MessageBus ports must be unique.");
	}
}