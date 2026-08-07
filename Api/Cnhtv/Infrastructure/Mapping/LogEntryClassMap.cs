using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cnhtv.Infrastructure.Mapping;

public sealed class LogEntryClassMap : IEntityTypeConfiguration<LogEntry>
{
    public void Configure(EntityTypeBuilder<LogEntry> builder)
    {
        builder.ToTable("logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder
            .Property(x => x.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired()
            .HasColumnType("datetime2");
        builder
            .Property(x => x.EventId)
            .HasColumnName("eventId")
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnType("int");
        builder.Property(x => x.Level).HasColumnName("level").IsRequired();
        builder.Property(x => x.Logger).HasColumnName("logger").IsRequired();
        builder.Property(x => x.Message).HasColumnName("message").IsRequired();
        builder.Property(x => x.CallSite).HasColumnName("call_site").IsRequired();
        builder.Property(x => x.Exception).HasColumnName("exception").IsRequired(false);
        builder.Property(x => x.Source).HasColumnName("source").IsRequired();

        builder.HasIndex(x => new { x.Source, x.Timestamp });
    }
}
