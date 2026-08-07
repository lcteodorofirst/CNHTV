namespace Cnhtv.Domain.Models;

public class TvConnectionHistory : BaseEntity<long>
{
    private TvConnectionHistory() { }

    public long TvDeviceId { get; private set; }
    public TvDevice TvDevice { get; private set; } = null!;
    public DateTime ConnectedAtUtc { get; private set; }
    public DateTime LastHeartbeatAtUtc { get; private set; }
    public DateTime? DisconnectedAtUtc { get; private set; }
    public string? IpAddress { get; private set; }

    public static TvConnectionHistory Start(long tvDeviceId, DateTime utcNow, string? ipAddress) =>
        new()
        {
            TvDeviceId = tvDeviceId,
            ConnectedAtUtc = utcNow,
            LastHeartbeatAtUtc = utcNow,
            IpAddress = ipAddress
        };

    public void Touch(DateTime utcNow, string? ipAddress)
    {
        LastHeartbeatAtUtc = utcNow;
        IpAddress = ipAddress;
    }

    public void Close(DateTime utcNow)
    {
        LastHeartbeatAtUtc = utcNow < LastHeartbeatAtUtc ? LastHeartbeatAtUtc : utcNow;
        DisconnectedAtUtc = utcNow;
    }
}
