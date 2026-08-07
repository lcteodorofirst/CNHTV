namespace Cnhtv.Infrastructure.Services;

using Cnhtv.Application.Common;
using Cnhtv.Application.Dtos.TvDevice;
using Cnhtv.Application.Interfaces.Devices;
using Cnhtv.Domain.Models;
using Cnhtv.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

public sealed class TvDeviceService(CnhtvDbContext dbContext) : ITvDeviceService
{
    public async Task<TvDeviceConfigurationDto> ConnectAsync(
        TvDeviceConnectDto dto,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var device = await dbContext.TvDevices
            .FirstOrDefaultAsync(x => x.DeviceKey == dto.DeviceKey, cancellationToken);

        if (device is null)
        {
            device = Cnhtv.Domain.Models.TvDevice.Create(
                dto.DeviceKey,
                dto.Name,
                DisplayDefaults.PresentationUrl,
                DisplayDefaults.RefreshEnabled,
                DisplayDefaults.RefreshIntervalSeconds,
                now);
            device.Heartbeat(dto.Model, dto.WebOsVersion, dto.AppVersion, ipAddress, now);
            dbContext.TvDevices.Add(device);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            device.Heartbeat(dto.Model, dto.WebOsVersion, dto.AppVersion, ipAddress, now);
        }

        await TouchSessionAsync(device.Id, now, ipAddress, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToConfiguration(device);
    }

    public async Task<TvDeviceConfigurationDto?> HeartbeatAsync(
        string deviceKey,
        TvDeviceHeartbeatDto dto,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var device = await dbContext.TvDevices
            .FirstOrDefaultAsync(x => x.DeviceKey == deviceKey, cancellationToken);
        if (device is null)
            return null;

        device.Heartbeat(dto.Model, dto.WebOsVersion, dto.AppVersion, ipAddress, now);
        await TouchSessionAsync(device.Id, now, ipAddress, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToConfiguration(device);
    }

    public async Task<TvDevice?> UpdateAsync(
        long id,
        TvDeviceUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var device = await dbContext.TvDevices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (device is null)
            return null;

        device.UpdateConfiguration(
            dto.Name,
            dto.PresentationUrl,
            dto.RefreshEnabled,
            dto.RefreshIntervalSeconds,
            DateTime.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return device;
    }

    private async Task TouchSessionAsync(
        long deviceId,
        DateTime now,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var session = await dbContext.TvConnectionHistory
            .FirstOrDefaultAsync(
                x => x.TvDeviceId == deviceId && x.DisconnectedAtUtc == null,
                cancellationToken);
        if (session is null)
            dbContext.TvConnectionHistory.Add(TvConnectionHistory.Start(deviceId, now, ipAddress));
        else
            session.Touch(now, ipAddress);
    }

    private static TvDeviceConfigurationDto ToConfiguration(Cnhtv.Domain.Models.TvDevice device) =>
        new(
            device.Id,
            device.DeviceKey,
            device.Name,
            device.PresentationUrl,
            device.RefreshEnabled,
            device.RefreshIntervalSeconds,
            device.ConfigurationUpdatedAtUtc,
            DisplayDefaults.HeartbeatIntervalSeconds);
}
