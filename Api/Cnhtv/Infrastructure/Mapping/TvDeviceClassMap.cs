namespace Cnhtv.Infrastructure.Mapping;

using Cnhtv.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TvDeviceClassMap : IEntityTypeConfiguration<TvDevice>
{
    public void Configure(EntityTypeBuilder<TvDevice> builder)
    {
        builder.ToTable("TvDevice");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.DeviceKey).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.DeviceKey).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Model).HasMaxLength(100);
        builder.Property(x => x.WebOsVersion).HasMaxLength(50);
        builder.Property(x => x.AppVersion).HasMaxLength(30);
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.Property(x => x.PresentationUrl).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.RefreshIntervalSeconds).IsRequired();
        builder.Property(x => x.RefreshEnabled).IsRequired();
        builder.Property(x => x.IsOnline).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasPrecision(3).IsRequired();
        builder.Property(x => x.LastSeenAtUtc).HasPrecision(3).IsRequired();
        builder.Property(x => x.ConfigurationUpdatedAtUtc).HasPrecision(3).IsRequired();
    }
}
