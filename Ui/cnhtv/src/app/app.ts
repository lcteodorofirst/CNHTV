import { Component, OnInit, signal } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  imports: [RouterOutlet],
})
export class App implements OnInit {
  protected readonly title = signal('CNH TV');

  constructor(
    private readonly appTitle: Title,
    private readonly translateService: TranslateService,
  ) {}

  public ngOnInit(): void {
    this.appTitle.setTitle(this.title());
    this.translateService.use('pt');
  }
}
