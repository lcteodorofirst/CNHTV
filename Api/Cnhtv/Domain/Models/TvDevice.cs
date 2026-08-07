namespace Cnhtv.Domain.Models;

public class TvDevice : BaseEntity<long>
{
    private TvDevice() { }

    public string DeviceKey { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Model { get; private set; }
    public string? WebOsVersion { get; private set; }
    public string? AppVersion { get; private set; }
    public string? IpAddress { get; private set; }
    public string PresentationUrl { get; private set; } = string.Empty;
    public bool RefreshEnabled { get; private set; }
    public int RefreshIntervalSeconds { get; private set; }
    public bool IsOnline { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime LastSeenAtUtc { get; private set; }
    public DateTime ConfigurationUpdatedAtUtc { get; private set; }

    public ICollection<TvConnectionHistory> ConnectionHistory { get; private set; } = [];

    public static TvDevice Create(
        string deviceKey,
        string? name,
        string presentationUrl,
        bool refreshEnabled,
        int refreshIntervalSeconds,
        DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(deviceKey))
            throw new ArgumentException("A identificação do equipamento é obrigatória.", nameof(deviceKey));

        var device = new TvDevice
        {
            DeviceKey = deviceKey.Trim(),
            Name = string.IsNullOrWhiteSpace(name)
                ? $"TV {deviceKey.Substring(Math.Max(0, deviceKey.Length - 6))}"
                : name.Trim(),
            CreatedAtUtc = utcNow,
            LastSeenAtUtc = utcNow,
            ConfigurationUpdatedAtUtc = utcNow,
            IsOnline = true
        };
        device.UpdateConfiguration(device.Name, presentationUrl, refreshEnabled, refreshIntervalSeconds, utcNow);
        return device;
    }

    public void UpdateConfiguration(
        string name,
        string presentationUrl,
        bool refreshEnabled,
        int refreshIntervalSeconds,
        DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome da TV é obrigatório.", nameof(name));
        if (!Uri.TryCreate(presentationUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Informe uma URL HTTP ou HTTPS válida.", nameof(presentationUrl));
        if (refreshIntervalSeconds < 30 || refreshIntervalSeconds > 86400)
            throw new ArgumentOutOfRangeException(nameof(refreshIntervalSeconds), "O refresh deve estar entre 30 segundos e 24 horas.");

        Name = name.Trim();
        PresentationUrl = presentationUrl.Trim();
        RefreshEnabled = refreshEnabled;
        RefreshIntervalSeconds = refreshIntervalSeconds;
        ConfigurationUpdatedAtUtc = utcNow;
    }

    public void Heartbeat(string? model, string? webOsVersion, string? appVersion, string? ipAddress, DateTime utcNow)
    {
        Model = TrimTo(model, 100);
        WebOsVersion = TrimTo(webOsVersion, 50);
        AppVersion = TrimTo(appVersion, 30);
        IpAddress = TrimTo(ipAddress, 64);
        LastSeenAtUtc = utcNow;
        IsOnline = true;
    }

    public void MarkOffline() => IsOnline = false;

    private static string? TrimTo(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var trimmed = value.Trim();
        return trimmed.Substring(0, Math.Min(trimmed.Length, maxLength));
    }
}
