import { Component ,computed,Input, ViewChild} from '@angular/core';
import { PriceListItemTableComponent } from '../price-list-item-table/price-list-item-table';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { Router } from '@angular/router';

@Component({
  selector: 'app-price-list-item-container',
  imports: [MatTableModule,FormsModule,MatButtonModule,MatMenuModule,MatIconModule, MatCheckboxModule,MatProgressSpinnerModule,MatInputModule, MatSortModule,MatPaginatorModule, CommonModule,PriceListItemTableComponent],
  templateUrl: './price-list-item-container.html',
  styleUrl: './price-list-item-container.scss'
})
export class PriceListItemContainerComponent {
  @Input() context: 'product' | 'pharmacy' = 'product';
  @Input() entityId!: string;

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

  applyFilter() {
    this.priceListComponent.searchKeyword = this.searchKeyword;
    this.priceListComponent.applyFilter();
  }

  triggerBulkAction(action: 'activate' | 'deactivate' | 'delete') {
  if (this.priceListComponent) {
    this.priceListComponent.handleAction(action, true);
  }
}

addPriceListItem() {
  if (this.context && this.entityId) {
    this.router.navigate(['/pricelistitem/add'], {
      queryParams: {
        context: this.context,
        entityId: this.entityId
      }
    });
  } else {
    this.router.navigate(['/pricelistitem/add']);
  }
}
}
