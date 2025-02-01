using Microsoft.EntityFrameworkCore;
using Server.Application.Services;
using Server.Domain.Library;
using System.Reflection;

namespace Server.DataApplicationLayer;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public sealed class ApplicationDbContext<TEntity>: DbContext where TEntity : Message
{
	private readonly string _connectionString;

	/// <inheritdoc cref="ApplicationDbContext{TEntity}"/>>
	public ApplicationDbContext(IConfigService configService)
	{
		_connectionString = configService
			.GetConfiguration()
			.ConnectionString;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseNpgsql(_connectionString);

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
			.Where(type => !string.IsNullOrEmpty(type.Namespace))
			.Where(type => type.GetInterfaces()
				.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

		foreach (var type in typesToRegister)
		{
			dynamic configurationInstance = Activator.CreateInstance(type) ?? throw new InvalidOperationException();
			modelBuilder.ApplyConfiguration(configurationInstance);
		}

		base.OnModelCreating(modelBuilder);
	}
}