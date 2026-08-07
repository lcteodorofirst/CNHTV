import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { TvConnectionHistory, TvDevice } from '../models/tv-device.model';

interface ODataResult<T> {
  value: T[];
  '@odata.count'?: number;
}

export interface TvDeviceUpdate {
  Name: string;
  PresentationUrl: string;
  RefreshEnabled: boolean;
  RefreshIntervalSeconds: number;
}

@Injectable({ providedIn: 'root' })
export class TvDeviceEndpoint {
  private readonly endpoint = `${environment.apiUrl}/odata/TvDevice`;

  public async list(top = 15, skip = 0): Promise<{ items: TvDevice[]; total: number }> {
    const response = await this.request<ODataResult<TvDevice>>(
      `${this.endpoint}?$count=true&$top=${top}&$skip=${skip}&$orderby=Name`,
    );
    return { items: response.value ?? [], total: response['@odata.count'] ?? response.value?.length ?? 0 };
  }

  public update(id: number, body: TvDeviceUpdate): Promise<TvDevice> {
    return this.request<TvDevice>(`${this.endpoint}/${id}`, 'PATCH', body);
  }

  public async history(deviceId: number): Promise<TvConnectionHistory[]> {
    const filter = encodeURIComponent(`TvDeviceId eq ${deviceId}`);
    const response = await this.request<ODataResult<TvConnectionHistory>>(
      `${environment.apiUrl}/odata/TvConnectionHistory?$filter=${filter}&$orderby=ConnectedAtUtc desc&$top=100`,
    );
    return response.value ?? [];
  }

  private async request<T>(url: string, method = 'GET', body?: unknown): Promise<T> {
    const response = await fetch(url, {
      method,
      headers:
        body === undefined
          ? { Accept: 'application/json' }
          : { Accept: 'application/json', 'Content-Type': 'application/json' },
      body: body === undefined ? undefined : JSON.stringify(body),
    });
    if (!response.ok) throw new Error((await response.text()) || `HTTP ${response.status}`);
    return (await response.json()) as T;
  }
}
