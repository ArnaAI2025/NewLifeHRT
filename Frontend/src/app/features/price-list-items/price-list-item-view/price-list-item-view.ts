import { Component, computed, ViewChild } from '@angular/core';
import { PriceListItemTableComponent } from '../price-list-item-table/price-list-item-table';
import { MatTableModule } from '@angular/material/table';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { Router } from '@angular/router';

@Component({
  selector: 'app-price-list-item-view',
  imports: [PriceListItemTableComponent,MatTableModule,FormsModule,MatButtonModule,MatMenuModule,MatIconModule, MatCheckboxModule,MatProgressSpinnerModule,MatInputModule, MatSortModule,MatPaginatorModule, CommonModule],
  templateUrl: './price-list-item-view.html',
  styleUrl: './price-list-item-view.scss'
})
export class PriceListItemViewComponent {
@ViewChild('priceListTable') priceListComponent!: PriceListItemTableComponent;
searchKeyword = '';

constructor(
    private router: Router,
  ) {}

get selectedCount(): number {
    return this.priceListComponent?.selectedCount ?? 0;
  }

  // 🔹 Angular will re-run computed only when its dependencies change
  isLoading = computed(() => this.priceListComponent?.isLoading() ?? false);

  // 🔹 Proxy Method for hasSelectedRows
  hasSelectedRows(): boolean {
    return this.priceListComponent?.hasSelectedRows?.() ?? false;
  }

  addPriceListItem() {
  this.router.navigate(['/pricelistitem/add']);
}

  applyFilter() {
    this.priceListComponent.searchKeyword = this.searchKeyword;
    this.priceListComponent.applyFilter();
  }

  triggerBulkAction(action: 'activate' | 'deactivate' | 'delete') {
  if (this.priceListComponent) {
    this.priceListComponent.handleAction(action, true);
  }
}

}
