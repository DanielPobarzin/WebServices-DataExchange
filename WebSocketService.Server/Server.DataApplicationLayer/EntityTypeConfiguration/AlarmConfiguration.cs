using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Messages;

namespace Server.DataApplicationLayer.EntityTypeConfiguration
{
	internal class AlarmConfiguration :  IEntityTypeConfiguration<Alarm>
	{
		public void Configure(EntityTypeBuilder<Alarm> builder)
		{
			builder.ToTable("messages_by_diagnostic_system");

			builder.HasNoKey();

			builder.Property(e => e.Content)
				.HasColumnName("content")
				.IsRequired()
				.HasMaxLength(500);

			builder.Property(e => e.TimeStamp)
				.HasColumnName("time_stamp")
				.IsRequired();

			builder.Property(e => e.Quality)
				.HasColumnName("quality")
				.IsRequired();

			builder.Property(e => e.Value)
				.HasColumnName("value")
				.IsRequired();

			builder.Property(e => e.TimeStamp)
				.HasDefaultValueSql("CURRENT_TIMESTAMP");

		}
	}
}
