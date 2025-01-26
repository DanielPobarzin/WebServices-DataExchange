using System.Reflection;
using Client.Domain.Library;
using Microsoft.EntityFrameworkCore;

namespace Client.DataApplicationLayer
{
	public sealed class ApplicationDbContext<TEntity>: MessagesDbContext where TEntity : Message
	{
		private readonly string _connectionString;

		/// <inheritdoc />
		public ApplicationDbContext(string connectionString)
		{
			_connectionString = connectionString;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseNpgsql(_connectionString);
	}

	public class MessagesDbContext : DbContext
	{
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
}
