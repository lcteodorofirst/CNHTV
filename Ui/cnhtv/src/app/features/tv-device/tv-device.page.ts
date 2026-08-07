import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { PageEvent } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';
import { TvDeviceEndpoint } from '../../domain/endpoints/tv-device.endpoint';
import { TvDevice } from '../../domain/models/tv-device.model';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { TableComponent } from '../../shared/components/table/table.component';
import { TvDeviceFormDialog } from './tv-device-form-dialog/tv-device-form.dialog';
import { TvHistoryDialog } from './tv-history-dialog/tv-history.dialog';

@Component({
  selector: 'app-tv-device-page',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    TranslatePipe,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    PageHeaderComponent,
    TableComponent,
  ],
  templateUrl: './tv-device.page.html',
  styleUrl: './tv-device.page.scss',
})
export class TvDevicePage implements OnInit, OnDestroy {
  private readonly endpoint = inject(TvDeviceEndpoint);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private refreshTimer?: ReturnType<typeof setInterval>;

  public readonly displayedColumns = [
    'Status',
    'Name',
    'Model',
    'IpAddress',
    'PresentationUrl',
    'LastSeenAtUtc',
    'actions',
  ];
  public devices: TvDevice[] = [];
  public loading = false;
  public totalItems = 0;
  public pageSize = 15;
  public pageIndex = 0;

  public ngOnInit(): void {
    void this.load();
    this.refreshTimer = setInterval(() => void this.load(false), 30000);
  }

  public ngOnDestroy(): void {
    if (this.refreshTimer) clearInterval(this.refreshTimer);
  }

  public async load(showLoading = true): Promise<void> {
    if (showLoading) this.loading = true;
    try {
      const result = await this.endpoint.list(this.pageSize, this.pageIndex * this.pageSize);
      this.devices = result.items;
      this.totalItems = result.total;
    } catch {
      this.snackBar.open('Não foi possível carregar as TVs.', 'Fechar', {
        duration: 5000,
        panelClass: ['error-snackbar'],
      });
    } finally {
      this.loading = false;
    }
  }

  public openConfiguration(device: TvDevice): void {
    this.dialog
      .open(TvDeviceFormDialog, { width: '42rem', maxWidth: '95vw', data: device })
      .afterClosed()
      .subscribe((reload) => reload === true && void this.load());
  }

  public openHistory(device: TvDevice): void {
    this.dialog.open(TvHistoryDialog, { width: '62rem', maxWidth: '95vw', data: device });
  }

  public onPageChange = (event: PageEvent): void => {
    this.pageSize = event.pageSize;
    this.pageIndex = event.pageIndex;
    void this.load();
  };
}
