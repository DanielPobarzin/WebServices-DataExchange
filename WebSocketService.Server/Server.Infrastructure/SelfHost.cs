using FunctionalProgramming;
using LanguageExt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Application.ConfigurationApp;
using Server.Application.Services;
using Server.Domain.Messages;
using Server.Infrastructure.Connection;
using Server.Infrastructure.Controllers;
using Server.Infrastructure.Hubs;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using Server.Application.Interfaces;

namespace Server.Infrastructure
{
	/// <inheritdoc cref="ISelfHost"/>
	public sealed class SelfHost : ISelfHost, IObserver<AppSetting>
	{
		#region fields

		private readonly ILogService _logger;

		#endregion

		private IHost? _host;

		/// <inheritdoc cref="SelfHost"/>
		public SelfHost(ILogService logger, IConfigService configService)
		{
			_logger = logger;
			configService.Subscribe(this);
		}
		#region implementation of ISelfHost

		public Task RebootHostedServiceAsync(AppSetting setting, CancellationToken cancellationToken = default) => ResultExtensions.TryCatchAsync(
			async () =>
			{
				if (_host is null)
					return StartHostedServiceAsync(setting, cancellationToken);

				await StopHostedServiceAsync(cancellationToken);
				return StartHostedServiceAsync(setting, cancellationToken);
			}, cancellationToken).MatchErrorAsync(async ex =>
			await _logger.Write(new LogError(ex, "SelfHostService", ex.Message)), token: cancellationToken);

		public Task StartHostedServiceAsync(AppSetting setting, CancellationToken cancellationToken = default) => ResultExtensions.TryCatchAsync(
			async () =>
			{
				_host = await CreateSelfHost(setting);
				await _host.StartAsync(cancellationToken);
				await Task.WhenAll(_logger.Write(new LogMessage("Hosted service has been started.", "SelfHostService")),
					_host.WaitForShutdownAsync(cancellationToken));

			}, cancellationToken).MatchErrorAsync(async ex =>
			await _logger.Write(new LogError(ex, $"SelfHostService.Method:{nameof(StartHostedServiceAsync)}", ex.Message)), token: cancellationToken);


		public Task StopHostedServiceAsync(CancellationToken cancellationToken = default) => ResultExtensions.TryCatchAsync(
			async () =>
			{
				if (_host is null)
					throw new InvalidOperationException();

				await _host.StopAsync(cancellationToken);
				_host.Dispose();
			}, cancellationToken).MatchErrorAsync(async ex =>
			await _logger.Write(new LogError(ex, "SelfHostService", ex.Message)), token: cancellationToken);


		#endregion

		private static Task<IHost> CreateSelfHost(AppSetting setting) =>
			Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((_, config) =>
				{
					config.AddEnvironmentVariables();
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseUrls(setting.GetUrls);
					webBuilder.ConfigureServices(services =>
						{
							services.AddCors(options =>
							{
								options.AddPolicy("AllowAll",
									builder => builder.AllowAnyMethod()
										.AllowAnyHeader()
										.WithExposedHeaders()
										.AllowAnyOrigin());
							});
							services.AddAuthentication(options =>
								{
									options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
									options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
								})
								.AddJwtBearer(options =>
								{
									options.TokenValidationParameters = new TokenValidationParameters
									{
										ValidateIssuer = true,
										ValidIssuer = AuthOptions.Issuer,
										ValidateAudience = true,
										ValidAudience = AuthOptions.Audience,
										ValidateLifetime = true,
										IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
										ValidateIssuerSigningKey = true
									};
									options.Events = new JwtBearerEvents
									{
										OnMessageReceived = context =>
										{
											var authRequest = (AccessToken: context.Request.Query["access_token"],
												context.HttpContext.Request.Path);
											if (!string.IsNullOrEmpty(authRequest.AccessToken) &&
											    authRequest.Path.StartsWithSegments(setting.RouteHub))
												context.Token = authRequest.AccessToken;

											return Task.CompletedTask;
										}
									};
								});
							services.AddAuthorization();
							services.AddControllers();
							services.AddSingleton<ILogService, LogService>();
							services.AddSingleton<IConnectionManager, ConnectionManager>();
							services.AddEndpointsApiExplorer();
							services.AddSignalR(configure =>
							{
								configure.EnableDetailedErrors = true;
								configure.KeepAliveInterval = TimeSpan.FromMinutes(1);
								configure.HandshakeTimeout = TimeSpan.FromSeconds(15);

							}).AddJsonProtocol(options => options.PayloadSerializerOptions.WriteIndented = true);
							services.AddSwaggerGen(c =>
							{
								var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
								var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
								c.IncludeXmlComments(xmlCommentsFullPath);

								c.SwaggerDoc("v1",
									new OpenApiInfo
									{
										Title = "SignalR WebSocket API",
										Version = "v1",
										Description = "SelfHost SignalR.WebSockets service",
										Contact = new OpenApiContact
										{
											Name = "C9Morty",
											Email = "daniil.nyashka@gmail.com",
											Url = new Uri("https://t.me/C9Morty")
										},
									});

								c.AddSecurityDefinition($"AuthToken v1",
									new OpenApiSecurityScheme
									{
										In = ParameterLocation.Header,
										Type = SecuritySchemeType.Http,
										BearerFormat = "JWT",
										Scheme = "bearer",
										Name = "Authorization",
										Description = "Authorization token"
									});

								c.AddSecurityRequirement(new OpenApiSecurityRequirement
								{
									{
										new OpenApiSecurityScheme
										{
											Reference = new OpenApiReference
											{
												Type = ReferenceType.SecurityScheme,
												Id = "AuthToken v1"
											}
										},
										[]
									}
								});

								c.AddSignalRSwaggerGen(opt =>
								{
									opt.UseHubXmlCommentsSummaryAsTag = true;
									opt.UseHubXmlCommentsSummaryAsTagDescription = true;
									opt.UseXmlComments(xmlCommentsFullPath);
								});

								c.CustomOperationIds(apiDescription =>
									apiDescription.TryGetMethodInfo(out var methodInfo)
										? methodInfo.Name
										: null);
							});
							services.AddSwaggerGenNewtonsoftSupport();
						})
						.Configure(app =>
						{
							app.UseHttpsRedirection();
							app.UseSwagger();
							app.UseSwaggerUI(c =>
							{
								c.SwaggerEndpoint("/swagger/v1/swagger.json", "SignalR WebSocket API");
								c.DocumentTitle = "WebSocket service";
								c.DocExpansion(DocExpansion.None);
							});

							app.UseRouting();
							app.UseAuthentication();
							app.UseAuthorization();
							app.UseCors("AllowAll");
							app.UseEndpoints(endpoints =>
							{
								endpoints.MapControllers();

								endpoints.MapHub<ServerHub>(setting.RouteHub, options =>
								{
									options.Transports = HttpTransportType.WebSockets;
									options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(setting.CloseTimeout);
									options.TransportSendTimeout = TimeSpan.FromSeconds(15);
								});
							});

						});
				}).Build().AsTask();

		public void OnCompleted() =>
			_logger.Write(new LogMessage("New configuration is pulled.",
				nameof(SelfHost)));

		public void OnError(Exception error) =>
			_logger.Write(new LogError(error,
				nameof(ConfigService),
				error.Message));

		public void OnNext(AppSetting value) => 
			RebootHostedServiceAsync(value);
	}

}
