import { BaseEntity } from '../base-entity';

export class TvDevice implements BaseEntity<number> {
  public Id = 0;
  public DeviceKey = '';
  public Name = '';
  public Model?: string;
  public WebOsVersion?: string;
  public AppVersion?: string;
  public IpAddress?: string;
  public PresentationUrl = '';
  public RefreshEnabled = false;
  public RefreshIntervalSeconds = 300;
  public IsOnline = false;
  public CreatedAtUtc = '';
  public LastSeenAtUtc = '';
  public ConfigurationUpdatedAtUtc = '';
}

export interface TvConnectionHistory {
  Id: number;
  TvDeviceId: number;
  ConnectedAtUtc: string;
  LastHeartbeatAtUtc: string;
  DisconnectedAtUtc?: string | null;
  IpAddress?: string | null;
}
