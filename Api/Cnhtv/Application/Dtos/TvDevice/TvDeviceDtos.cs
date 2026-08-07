namespace Cnhtv.Application.Dtos.TvDevice;

public sealed record TvDeviceConnectDto(
    string DeviceKey,
    string? Name,
    string? Model,
    string? WebOsVersion,
    string? AppVersion);

public sealed record TvDeviceHeartbeatDto(
    string? Model,
    string? WebOsVersion,
    string? AppVersion);

public sealed record TvDeviceUpdateDto(
    string Name,
    string PresentationUrl,
    bool RefreshEnabled,
    int RefreshIntervalSeconds);

public sealed record TvDeviceConfigurationDto(
    long Id,
    string DeviceKey,
    string Name,
    string PresentationUrl,
    bool RefreshEnabled,
    int RefreshIntervalSeconds,
    DateTime ConfigurationUpdatedAtUtc,
    int HeartbeatIntervalSeconds);
