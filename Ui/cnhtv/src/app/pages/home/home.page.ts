import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import { environment } from '../../../environments/environment';
import { TvDevice } from '../../domain/models/tv-device.model';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, PageHeaderComponent, TranslatePipe, MatIconModule, MatButtonModule],
  templateUrl: './home.page.html',
  styleUrl: './home.page.scss',
})
export class HomePage implements OnInit, OnDestroy {
  private timer?: ReturnType<typeof setInterval>;
  public devices: TvDevice[] = [];
  public loading = true;

  public get online(): number {
    return this.devices.filter((item) => item.IsOnline).length;
  }
  public get offline(): number {
    return this.devices.length - this.online;
  }
  public get lastContacts(): TvDevice[] {
    return [...this.devices].sort((a, b) => b.LastSeenAtUtc.localeCompare(a.LastSeenAtUtc)).slice(0, 5);
  }

  public ngOnInit(): void {
    void this.load();
    this.timer = setInterval(() => void this.load(), 30000);
  }

  public async load(): Promise<void> {
    try {
      const response = await fetch(`${environment.apiUrl}/odata/TvDevice?$orderby=LastSeenAtUtc desc`);
      if (response.ok) this.devices = (await response.json()).value ?? [];
    } finally {
      this.loading = false;
    }
  }

  public ngOnDestroy(): void {
    if (this.timer) clearInterval(this.timer);
  }
}
