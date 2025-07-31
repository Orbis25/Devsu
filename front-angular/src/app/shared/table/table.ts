import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IPaginationResult } from '../../../shared/interfaces/pagination.interface';
import { CommonModule } from '@angular/common';


export interface Column<T> {
  key: keyof T | 'actions';
  label: string;
  isButtons?:boolean
  render?: (value: unknown, row: T) => any;
}

@Component({
  selector: 'app-table',
  standalone: false,
  templateUrl: './table.html',
  styleUrl: './table.css',
})
export class TableComponent<T> {
  @Input() columns: Column<T>[] = [];
  @Input() data: IPaginationResult<T> = { results: [], total: 0, pageTotal: 0, qyt: 0, actualPage:0 };
  @Input() currentPage: number = 1;
  @Input() hideActions: boolean = false;
  @Input() isButtons: boolean = false;
  @Output() rowClick = new EventEmitter<T>();
  @Output() changePage = new EventEmitter<number>();
  @Output() actionClick = new EventEmitter<{ action: string, row: T }>();

  getColumns(): Column<T>[] {
    if (this.hideActions) {
      return this.columns.filter(col => col.key !== 'actions');
    }
    return this.columns;
  }

  handleChangePage(page: number): void {
    this.changePage.emit(page);
  }

  onRowClick(row: T): void {
    this.rowClick.emit(row);
  }

  get paginationButtons(): number[] {
    return Array.from({ length: this.data.pageTotal }, (_, i) => i + 1);
  }

  renderCell(col: Column<T>, row: T): any {
    if (col.render) {
      return col.render(null, row);
    }
    return String(row[col.key as keyof T]);
  }

  handleActionClick(action: string, row: T): void {
  
    if (action) {
      this.actionClick.emit({ action, row });
    }
  }
}
