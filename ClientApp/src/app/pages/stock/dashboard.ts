import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { StockService, StockItemListDto, StockPagedResponse, StockItem } from '../../services/stock.service';
import { MatDialog } from '@angular/material/dialog';
import { StockForm, StockFormData } from './stock-form';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router } from '@angular/router';
import {  Sort } from '@angular/material/sort';
import { MatSortModule } from '@angular/material/sort';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatInputModule,
    MatSnackBarModule,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatSortModule
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class Dashboard implements OnInit {
  displayedColumns: string[] = ['primaryImage', 'make', 'model', 'modelYear', 'regNo', 'colour', 'vin', 'retailPrice', 'actions'];
  dataSource: StockItemListDto[] = [];

  // Pagination
  page = 1;
  pageSize = 10;
  totalItems = 0;

  sortBy: string = 'DTUpdated';
  sortDir: 'asc' | 'desc' = 'desc';

  // Search
  searchTerm: string = '';

  constructor(
    private stockService: StockService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private router: Router,
    ) {}

  ngOnInit() {
    this.loadData();
  }

onSortChange(sort: Sort | any) {
  const s = sort as Sort;
  this.sortBy = s.active;
  this.sortDir = (s.direction === 'asc' || s.direction === 'desc') ? s.direction : 'asc';
  this.page = 1;
  this.loadData();
}

  loadData() {
    this.stockService
      .getPaged(this.page, this.pageSize, this.sortBy, this.sortDir, this.searchTerm)
      .subscribe((res: StockPagedResponse) => {
        this.dataSource = res.data;
        this.totalItems = res.total;
      });
  }

  onPageChange(event: PageEvent) {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  onSearch() {
    this.page = 1; // reset to first page when searching
    this.loadData();
  }

  onLogout() {
    localStorage.removeItem('token'); 
    this.router.navigate(['/login']);
  }

    // This opens the Add/Edit stock dialog
  openStockForm(stockItem?: StockItem) {
      const dialogRef = this.dialog.open<StockForm, StockFormData>(StockForm, {
        width: '700px', 
         maxWidth : '700px',
        data: { stockItem } 
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if (result === 'saved') {
          this.loadData(); // refresh table after add/edit
        }
      });
    }

    // Edit stock item
editStock(stockItem: StockItem) {
  this.stockService.getById(stockItem.id).subscribe({  
    next: (fullStockItem) => {
      const dialogRef = this.dialog.open(StockForm, {
        width: '700px',  
        maxWidth : '700px',
        data: { stockItem: fullStockItem }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result === 'saved') {
          this.loadData();
        }
      });
    },
    error: (err) => {
      console.error(err);
      this.snackBar.open('Failed to load stock item', '', { duration: 2000 });
    }
  });
}
  
  // Delete stock item
  deleteStock(item: StockItem) {
    if (confirm(`Are you sure you want to delete ${item.make} ${item.model}?`)) {
      this.stockService.deleteStock(item.id).subscribe({
        next: () => {
          this.snackBar.open('Stock deleted successfully', '', { duration: 1500 });
          this.loadData(); // refresh table
        },
        error: err => {
          console.error(err);
          this.snackBar.open('Failed to delete stock', '', { duration: 2000 });
        }
      });
    }
  }
}
