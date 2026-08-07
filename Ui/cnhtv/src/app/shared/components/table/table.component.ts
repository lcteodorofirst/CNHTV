import {
  AfterContentInit,
  Component,
  ContentChildren,
  Input,
  OnChanges,
  QueryList,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatColumnDef, MatTable, MatTableDataSource, MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss',
  standalone: true,
  imports: [MatPaginatorModule, MatProgressBarModule, MatTableModule, TranslatePipe],
})
export class TableComponent<T> implements AfterContentInit, OnChanges {
  @Input({ required: true }) public items!: T[];
  @Input() public loading = false;
  @Input() public pageSize = 10;
  @Input() public pageSizeOptions: number[] = [5, 10, 15, 25, 50];
  @Input() public totalItems = 0;
  @Input() public headers: string[] = [];
  @Input({ required: true }) public pageChange!: (event: PageEvent) => void;

  @ContentChildren(MatColumnDef) public columnDefs!: QueryList<MatColumnDef>;
  @ViewChild(MatTable, { static: true }) public table!: MatTable<T>;
  public dataSource = new MatTableDataSource<T>([]);

  public ngOnChanges(changes: SimpleChanges): void {
    if (changes['items']) this.dataSource.data = this.items ?? [];
  }

  public ngAfterContentInit(): void {
    this.columnDefs.forEach((columnDef) => this.table.addColumnDef(columnDef));
  }
}
