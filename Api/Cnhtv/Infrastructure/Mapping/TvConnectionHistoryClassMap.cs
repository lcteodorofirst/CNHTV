namespace Cnhtv.Infrastructure.Mapping;

using Cnhtv.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TvConnectionHistoryClassMap : IEntityTypeConfiguration<TvConnectionHistory>
{
    public void Configure(EntityTypeBuilder<TvConnectionHistory> builder)
    {
        builder.ToTable("TvConnectionHistory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ConnectedAtUtc).HasPrecision(3).IsRequired();
        builder.Property(x => x.LastHeartbeatAtUtc).HasPrecision(3).IsRequired();
        builder.Property(x => x.DisconnectedAtUtc).HasPrecision(3);
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.HasIndex(x => new { x.TvDeviceId, x.DisconnectedAtUtc });
        builder.HasOne(x => x.TvDevice)
            .WithMany(x => x.ConnectionHistory)
            .HasForeignKey(x => x.TvDeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
