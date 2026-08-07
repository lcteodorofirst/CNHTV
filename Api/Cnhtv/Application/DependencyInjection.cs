namespace Cnhtv.Application.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.ModelBuilder;
using Cnhtv.Domain.Models;

public static partial class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        ODataConventionModelBuilder edmBuilder
    )
    {
        AddServices(services);

        AddEdmModels(services, edmBuilder);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddEdmModels(
        this IServiceCollection services,
        ODataConventionModelBuilder edmBuilder
    )
    {
        var tvDevice = edmBuilder.EntityType<TvDevice>();
        tvDevice.HasKey(x => x.Id);
        tvDevice.Property(x => x.DeviceKey);
        tvDevice.Property(x => x.Name);
        tvDevice.Property(x => x.Model);
        tvDevice.Property(x => x.WebOsVersion);
        tvDevice.Property(x => x.AppVersion);
        tvDevice.Property(x => x.IpAddress);
        tvDevice.Property(x => x.PresentationUrl);
        tvDevice.Property(x => x.RefreshEnabled);
        tvDevice.Property(x => x.RefreshIntervalSeconds);
        tvDevice.Property(x => x.IsOnline);
        tvDevice.Property(x => x.CreatedAtUtc);
        tvDevice.Property(x => x.LastSeenAtUtc);
        tvDevice.Property(x => x.ConfigurationUpdatedAtUtc);

        var connectionHistory = edmBuilder.EntityType<TvConnectionHistory>();
        connectionHistory.HasKey(x => x.Id);
        connectionHistory.Property(x => x.TvDeviceId);
        connectionHistory.Property(x => x.ConnectedAtUtc);
        connectionHistory.Property(x => x.LastHeartbeatAtUtc);
        connectionHistory.Property(x => x.DisconnectedAtUtc);
        connectionHistory.Property(x => x.IpAddress);

        edmBuilder.EntitySet<TvDevice>(nameof(TvDevice));
        edmBuilder.EntitySet<TvConnectionHistory>(nameof(TvConnectionHistory));

        return services;
    }
}
