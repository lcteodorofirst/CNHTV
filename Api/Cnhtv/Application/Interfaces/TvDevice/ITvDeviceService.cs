namespace Cnhtv.Application.Interfaces.Devices;

using Cnhtv.Application.Dtos.TvDevice;
using Cnhtv.Domain.Models;

public interface ITvDeviceService
{
    Task<TvDeviceConfigurationDto> ConnectAsync(
        TvDeviceConnectDto dto,
        string? ipAddress,
        CancellationToken cancellationToken);

    Task<TvDeviceConfigurationDto?> HeartbeatAsync(
        string deviceKey,
        TvDeviceHeartbeatDto dto,
        string? ipAddress,
        CancellationToken cancellationToken);

    Task<TvDevice?> UpdateAsync(
        long id,
        TvDeviceUpdateDto dto,
        CancellationToken cancellationToken);
}
