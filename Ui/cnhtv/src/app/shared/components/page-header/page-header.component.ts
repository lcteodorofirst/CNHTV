import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'page-header',
  standalone: true,
  templateUrl: './page-header.component.html',
  styleUrl: './page-header.component.scss',
  imports: [CommonModule, MatMenuModule, MatButtonModule],
})
export class PageHeaderComponent {
  @Input() public title: string = '';
  @Input() public buttonText?: string;
  @Input() public buttonRoles: string[] = ['Admin', 'Common'];
  @Output() public notifyParent = new EventEmitter();

  public emitClick() {
    this.notifyParent.emit();
  }
}
