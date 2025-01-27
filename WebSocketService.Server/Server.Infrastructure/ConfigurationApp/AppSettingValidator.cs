using FluentValidation;

namespace Server.Infrastructure.ConfigurationApp;

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
			.Matches(@"^host=.*;port=.*;Database=.*;Username=.*;Password=.*$").WithMessage("Connection string format is invalid.");

		RuleFor(x => x.Host)
			.NotEmpty().WithMessage("Host cannot be empty.")
			.Matches(@"^(localhost|[a-zA-Z0-9.-]+)$").WithMessage("Host must be a valid hostname or 'localhost'.");

		RuleFor(x => x.Protocol)
			.NotEmpty().WithMessage("Protocol cannot be empty.")
			.Must(protocol => protocol is "http" or "https").WithMessage("Protocol must be either 'http' or 'https'.");

		RuleFor(x => x.Port)
			.NotNull().WithMessage("Port cannot be null.")
			.Must(ports => ports.Length > 0).WithMessage("At least one port must be specified.")
			.Must(ports => ports.All(port => port is > 0 and <= 65535)).WithMessage("All ports must be valid (1-65535).");

		RuleFor(x => x.RouteHub)
			.NotEmpty().WithMessage("RouteHub cannot be empty.")
			.Matches(@"^/[a-zA-Z0-9/]*$").WithMessage("RouteHub must start with '/' and contain only alphanumeric characters and slashes.");

		RuleFor(x => x.CloseTimeout)
			.GreaterThan(0).WithMessage("CloseTimeout must be greater than zero.");

		RuleFor(x => x.MessageBusHost)
			.NotEmpty().WithMessage("MessageBusHost cannot be empty.")
			.Matches(@"^(localhost|[a-zA-Z0-9.-]+)$").WithMessage("MessageBus must be a valid hostname or 'localhost'.");

		RuleFor(x => x.MessageBusPort)
			.NotNull().WithMessage("MessageBusPort cannot be null.")
			.Must(kafkaPorts => kafkaPorts.Length > 0).WithMessage("At least one MessageBus port must be specified.")
			.Must(kafkaPorts => kafkaPorts.All(port => port is > 0 and <= 65535)).WithMessage("All MessageBus ports must be valid (1-65535).");
	}
}