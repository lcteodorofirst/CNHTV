import { CommonModule, DatePipe } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { TvDeviceEndpoint } from '../../../domain/endpoints/tv-device.endpoint';
import { TvConnectionHistory, TvDevice } from '../../../domain/models/tv-device.model';

@Component({
  selector: 'app-tv-history-dialog',
  standalone: true,
  imports: [CommonModule, DatePipe, MatDialogModule, MatButtonModule, MatProgressSpinnerModule, MatTableModule],
  templateUrl: './tv-history.dialog.html',
  styleUrl: './tv-history.dialog.scss',
})
export class TvHistoryDialog implements OnInit {
  public readonly columns = ['ConnectedAtUtc', 'DisconnectedAtUtc', 'duration', 'IpAddress'];
  public loading = true;
  public history: TvConnectionHistory[] = [];

  constructor(
    private readonly endpoint: TvDeviceEndpoint,
    @Inject(MAT_DIALOG_DATA) public readonly device: TvDevice,
  ) {}

  public async ngOnInit(): Promise<void> {
    try {
      this.history = await this.endpoint.history(this.device.Id);
    } finally {
      this.loading = false;
    }
  }

  public duration(item: TvConnectionHistory): string {
    const start = new Date(item.ConnectedAtUtc).getTime();
    const end = item.DisconnectedAtUtc ? new Date(item.DisconnectedAtUtc).getTime() : Date.now();
    const totalMinutes = Math.max(0, Math.floor((end - start) / 60000));
    return `${Math.floor(totalMinutes / 60)}h ${totalMinutes % 60}min`;
  }
}
